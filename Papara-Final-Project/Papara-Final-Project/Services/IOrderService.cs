using Papara_Final_Project.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrders();
        Task<OrderDTO> GetOrderById(int id);
        Task AddOrder(OrderDTO orderDto, PaymentDTO paymentDTO, int userId); 
        Task DeleteOrder(int id);
    }
}
