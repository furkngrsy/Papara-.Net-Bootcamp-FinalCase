using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public interface IOrderService
    {
        Order GetOrderById(int id);
        IEnumerable<Order> GetAllOrders();
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
    }
}
