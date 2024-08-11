using Papara_Final_Project.Models;

namespace Papara_Final_Project.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task<Order> GetOrderById(int id);
        Task AddOrder(Order order);
        Task UpdateOrder(Order order);
        Task DeleteOrder(int id);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
    }
}
