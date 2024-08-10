using Papara_Final_Project.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrders();
        Task<OrderWithDetailsDTO> GetOrderById(int id); // Sipariş Detayları DTO
        Task<List<OrderDetailExtendedDTO>> GetOrderProductDetails(int id); // Sipariş Ürün Bilgileri DTO
        Task AddOrder(OrderDTO orderDto, PaymentDTO paymentDto, int userId);
        Task UpdateOrder(int id, OrderDTO orderDto);
        Task DeleteOrder(int id);
        Task<IEnumerable<OrderWithDetailsDTO>> GetActiveOrders(int userId);
        Task<IEnumerable<OrderWithDetailsDTO>> GetInactiveOrders(int userId);

    }
}
