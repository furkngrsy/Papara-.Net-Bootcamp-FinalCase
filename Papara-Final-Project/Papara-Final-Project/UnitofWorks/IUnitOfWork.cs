using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using System.Threading.Tasks;

namespace Papara_Final_Project.UnitOfWorks
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        ICouponRepository Coupons { get; } // ICouponRepository ekleniyor
        DbSet<ProductMatchCategory> ProductMatchCategories { get; }
        Task<int> CompleteAsync();
    }
}
