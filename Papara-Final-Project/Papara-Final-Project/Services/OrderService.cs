using FluentValidation;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using Papara_Final_Project.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly IUserRepository _userRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IValidator<OrderDTO> _orderValidator;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IProductService productService, IUserRepository userRepository, ICouponRepository couponRepository, IValidator<OrderDTO> orderValidator, IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _userRepository = userRepository;
            _couponRepository = couponRepository;
            _orderValidator = orderValidator;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return orders.Select(o => new OrderDTO
            {
                CouponCode = o.CouponCode,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<OrderWithDetailsDTO> GetOrderById(int id) 
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return null;
            }

            var orderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity
            }).ToList();

            return new OrderWithDetailsDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                CouponCode = order.CouponCode,
                CouponAmount = order.CouponAmount,
                PointsUsed = order.PointsUsed,
                OrderDate = order.OrderDate
            };
        }

        public async Task<List<OrderDetailExtendedDTO>> GetOrderProductDetails(int id) 
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return null;
            }

            var productDetails = new List<OrderDetailExtendedDTO>();
            foreach (var detail in order.OrderDetails)
            {
                var product = await _productService.GetProductById(detail.ProductId);
                productDetails.Add(new OrderDetailExtendedDTO
                {
                    ProductId = detail.ProductId,
                    ProductName = product.Name,
                    Price = detail.Price,
                    Quantity = detail.Quantity,
                    TotalPrice = detail.Price * detail.Quantity
                });
            }

            return productDetails;
        }



        public async Task AddOrder(OrderDTO orderDto, PaymentDTO paymentDto, int userId)
        {
            var validationResult = await _orderValidator.ValidateAsync(orderDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            decimal totalProductAmount = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var detail in orderDto.OrderDetails)
            {
                if (!await _productService.IsProductAvailable(detail.ProductId, detail.Quantity))
                {
                    throw new Exception($"Product with ID {detail.ProductId} is not available in the requested quantity.");
                }
                var product = await _productService.GetProductById(detail.ProductId);
                decimal productPrice = product.Price;
                totalProductAmount += productPrice * detail.Quantity;

                // Ürün stoğunu düşürme
                product.Stock -= detail.Quantity;
                await _productService.UpdateProduct(detail.ProductId, product);

                orderDetails.Add(new OrderDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Price = productPrice
                });
            }

            // Kupon ve puan kullanımı
            decimal couponAmount = 0;
            if (!string.IsNullOrEmpty(orderDto.CouponCode))
            {
                var coupon = await _couponRepository.GetCouponByCode(orderDto.CouponCode);
                if (coupon == null)
                {
                    throw new Exception("Invalid coupon code.");
                }
                couponAmount = coupon.DiscountAmount;
                coupon.IsUsed = true;
                await _couponRepository.UpdateCoupon(coupon);
            }

            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            decimal pointsUsed = 0;
            decimal amountToPay = totalProductAmount - couponAmount;

            // Puan kullanımı
            if (amountToPay > 0)
            {
                pointsUsed = user.PointsBalance;
                amountToPay -= pointsUsed;
                if (amountToPay < 0)
                {
                    pointsUsed += amountToPay; // Kullanılabilecek maksimum puanı ayarlıyoruz
                    amountToPay = 0;
                }
            }

            // Eğer hala ödenmesi gereken tutar varsa, ödeme işlemi yapılır
            if (amountToPay > 0)
            {
                var paymentResult = await _paymentService.ProcessPayment(paymentDto);
                if (!paymentResult)
                {
                    throw new Exception("Payment failed. Please check your card details.");
                }
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalProductAmount,
                CouponCode = orderDto.CouponCode,
                CouponAmount = couponAmount,
                PointsUsed = pointsUsed,
                OrderDate = DateTime.Now,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddOrder(order);
            await _unitOfWork.CompleteAsync();

            // Kullanıcının cüzdan bakiyesinden puan düşme
            user.PointsBalance -= pointsUsed;
            if (user.PointsBalance < 0)
            {
                user.PointsBalance = 0;
            }

            // Kupon ve puanların ürünlere eşit dağıtılması
            int orderDetailCount = orderDetails.Count;
            decimal couponPerProduct = couponAmount / orderDetailCount;
            decimal pointsPerProduct = pointsUsed / orderDetailCount;

            // Kredi kartı ile ödenen tutar üzerinden puan kazanma
            decimal pointsEarned = 0;
            foreach (var detail in orderDetails.OrderByDescending(od => od.Price * od.Quantity))
            {
                var product = await _productService.GetProductById(detail.ProductId);
                if (product != null)
                {
                    decimal netPrice = (detail.Price * detail.Quantity) - pointsPerProduct - couponPerProduct;
                    decimal productPoints = (netPrice) * (product.RewardRate / 100);
                    pointsEarned += productPoints > product.MaxReward ? product.MaxReward : productPoints;
                }
                pointsPerProduct = 0;
                couponPerProduct = 0;
            }

            user.PointsBalance += pointsEarned;

            await _userRepository.UpdateUser(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateOrder(int id, OrderDTO orderDto)
        {
            var validationResult = await _orderValidator.ValidateAsync(orderDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            order.CouponCode = orderDto.CouponCode;
            order.OrderDetails = await Task.WhenAll(orderDto.OrderDetails.Select(async od => new OrderDetail
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                Price = await _productService.GetProductPriceById(od.ProductId)
            }));

            await _orderRepository.UpdateOrder(order);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteOrder(int id)
        {
            await _orderRepository.DeleteOrder(id);
            await _unitOfWork.CompleteAsync();
        }

    }
}
