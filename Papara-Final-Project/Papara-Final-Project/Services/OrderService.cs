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
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, IProductService productService, IUserRepository userRepository, ICouponRepository couponRepository, IValidator<OrderDTO> orderValidator, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _userRepository = userRepository;
            _couponRepository = couponRepository;
            _orderValidator = orderValidator;
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

        public async Task<OrderDTO> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return null;
            }

            return new OrderDTO
            {
                CouponCode = order.CouponCode,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity
                }).ToList()
            };
        }

        public async Task AddOrder(OrderDTO orderDto, int userId)
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
            }

            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            decimal pointsUsed = user.PointsBalance;
            decimal amountToPay = totalProductAmount - couponAmount - pointsUsed;
            if (amountToPay < 0)
            {
                pointsUsed += amountToPay; // Kullanılabilecek maksimum puanı ayarlıyoruz
                amountToPay = 0;
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

            // Kredi kartı ile ödenen tutar üzerinden puan kazanma
            decimal pointsEarned = 0;
            foreach (var detail in orderDetails)
            {
                var product = await _productService.GetProductById(detail.ProductId);
                if (product != null)
                {
                    decimal productPoints = (detail.Price * detail.Quantity) * (product.RewardRate / 100);
                    pointsEarned += productPoints > product.MaxReward ? product.MaxReward : productPoints;
                }
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
