using FluentValidation;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using Papara_Final_Project.UnitOfWorks;


namespace Papara_Final_Project.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly IUserRepository _userRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IPaymentService _paymentService;
        private readonly IValidator<OrderDTO> _orderValidator; 
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

            foreach (var detail in orderDto.OrderDetails)//siparişteki her bir ürünün kontrolü ve ürünün stok düşürülmesi.
            {
                if (!await _productService.IsProductAvailable(detail.ProductId, detail.Quantity))
                {
                    throw new Exception($"Product with ID {detail.ProductId} is not available.");
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

            // Kupon kullanımı
            decimal couponAmount = await ApplyCoupon(orderDto.CouponCode);
            
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Puan kullanımı
            // Eğer hala ödenmesi gereken tutar varsa, ödeme işlemi yapılır
            decimal pointsUsed = 0;
            decimal amountToPay = totalProductAmount - couponAmount;

            if (amountToPay > 0)
            {
                pointsUsed = await PointAndPayment(user, amountToPay, pointsUsed, paymentDto);
            }

            var order = CreateOrder(userId, totalProductAmount, orderDto.CouponCode, couponAmount, pointsUsed, orderDetails);//Order oluşturma.

            await _orderRepository.AddOrder(order);
            await _unitOfWork.CompleteAsync();

            // Kullanıcının cüzdan bakiyesinden puan düşme
            user.PointsBalance -= pointsUsed;
            if (user.PointsBalance < 0)
            {
                user.PointsBalance = 0;
            }

            decimal pointsEarned = await DistributeRewards(orderDetails, couponAmount, pointsUsed);//geri puan ödeme işlemi.
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

        public async Task<IEnumerable<OrderWithDetailsDTO>> GetActiveOrders(int userId)
        {
            var orders = await _orderRepository.GetAllOrders();
            var activeOrders = orders
                .Where(o => o.UserId == userId && o.OrderDate >= DateTime.Now.AddDays(-10))
                .ToList();

            if (!activeOrders.Any())
            {
                return null;
            }

            var activeOrderDtos = activeOrders.Select(o => new OrderWithDetailsDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                TotalAmount = o.TotalAmount,
                CouponCode = o.CouponCode,
                CouponAmount = o.CouponAmount,
                PointsUsed = o.PointsUsed,
                OrderDate = o.OrderDate,
            }).ToList();

            return activeOrderDtos;
        }

        public async Task<IEnumerable<OrderWithDetailsDTO>> GetInactiveOrders(int userId)
        {
            var orders = await _orderRepository.GetAllOrders();
            var inactiveOrders = orders
                .Where(o => o.UserId == userId && o.OrderDate < DateTime.Now.AddDays(-10))
                .ToList();

            if (!inactiveOrders.Any())
            {
                return null;
            }

            var inactiveOrderDtos = inactiveOrders.Select(o => new OrderWithDetailsDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                TotalAmount = o.TotalAmount,
                CouponCode = o.CouponCode,
                CouponAmount = o.CouponAmount,
                PointsUsed = o.PointsUsed,
                OrderDate = o.OrderDate,
            }).ToList();

            return inactiveOrderDtos;
        }

        //Kupon kontrolü yapan fonksiyon.
        private async Task<decimal> ApplyCoupon(string couponCode)
        {
            decimal couponAmount = 0;
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _couponRepository.GetCouponByCode(couponCode);
                if (coupon == null)
                {
                    throw new Exception("Invalid coupon code.");
                }
                couponAmount = coupon.DiscountAmount;
                coupon.IsUsed = true;
                await _couponRepository.UpdateCoupon(coupon);
            }
            return couponAmount;
        }


        //Puan hesaplaması yaparak ödenecek tutar var ise ödeme yaptıran fonksiyon.
        private async Task<decimal> PointAndPayment(User user, decimal amountToPay , decimal pointsUsed, PaymentDTO paymentDto)
        {
            pointsUsed = user.PointsBalance;
            amountToPay -= pointsUsed;
            if (amountToPay < 0)
            {
                pointsUsed += amountToPay; // Kullanılabilecek maksimum puanı ayarlıyoruz
                amountToPay = 0;
            }
            else if (amountToPay > 0) //Eğer puan çıkarıldığı taktirde hala ödenecek tutar var ise kart ödeme işlemini başlatan koşul.
            {
                await ProcessPayment(paymentDto);
            }

            return pointsUsed;
        }

        private async Task ProcessPayment(PaymentDTO paymentDto)
        {
            var paymentResult = await _paymentService.ProcessPayment(paymentDto);

            if (!paymentResult)
            {
                throw new Exception("Payment failed. Please check your card details.");
            }
        }

        private Order CreateOrder(int userId, decimal totalProductAmount, string couponCode, decimal couponAmount, decimal pointsUsed, List<OrderDetail> orderDetails)
        {
            return new Order
            {
                UserId = userId,
                TotalAmount = totalProductAmount,
                CouponCode = couponCode,
                CouponAmount = couponAmount,
                PointsUsed = pointsUsed,
                OrderDate = DateTime.Now,
                OrderDetails = orderDetails
            };
        }

        //kullanılan kupon ve puan miktarını eşit bir şekilde ürünlere dağıtıp geri puan ödemesini sağlayan fonksiyon.
        private async Task<decimal> DistributeRewards(List<OrderDetail> orderDetails, decimal couponAmount, decimal pointsUsed)
        {
            int orderDetailCount = orderDetails.Count;
            decimal couponPerProduct = couponAmount / orderDetailCount;
            decimal pointsPerProduct = pointsUsed / orderDetailCount;

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
            }
            return pointsEarned;
        }
    }
}
