using System;
using System.Threading.Tasks;

namespace Papara_Final_Project.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICouponRepository Coupons { get; }
        Task<int> CompleteAsync();
    }
}
