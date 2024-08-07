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

                product.Stock -= detail.Quantity;
                await _productService.UpdateProduct(detail.ProductId, product);

                orderDetails.Add(new OrderDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    Price = productPrice
                });
            }


            decimal couponAmount = 0;
            if (!string.IsNullOrEmpty(orderDto.CouponCode))
            {
                var coupon = await _couponRepository.GetCouponByCode(orderDto.CouponCode);
                if (coupon == null)
                {
                    throw new Exception("Invalid coupon code.");
                }

                if (coupon.IsUsed == true)
                {
                    throw new Exception("This coupon code is not available.");
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

            if (amountToPay > 0)
            {
                pointsUsed = user.PointsBalance;
                amountToPay -= pointsUsed;
                if (amountToPay < 0)
                {
                    pointsUsed += amountToPay; 
                    amountToPay = 0;
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

            user.PointsBalance -= pointsUsed;
            if (user.PointsBalance < 0)
            {
                user.PointsBalance = 0;
            }

            int orderDetailCount = orderDetails.Count;
            decimal couponPerProduct = couponAmount / orderDetailCount;
            decimal pointsPerProduct = pointsUsed / orderDetailCount;

            decimal pointsEarned = 0;
            foreach (var detail in orderDetails)
            {
                var product = await _productService.GetProductById(detail.ProductId);
                if (product != null)
                {
                    decimal netPrice = (detail.Price * detail.Quantity) - pointsPerProduct - couponPerProduct;
                    decimal productPoints = (netPrice) * (product.RewardRate / 100);
                    pointsEarned += productPoints > product.MaxReward ? product.MaxReward : productPoints;
                }
            }

            user.PointsBalance += pointsEarned;

            await _userRepository.UpdateUser(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteOrder(int id)
        {
            await _orderRepository.DeleteOrder(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
