using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;
using System.Threading.Tasks;

namespace Papara_Final_Project.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        DbSet<ProductMatchCategory> ProductMatchCategories { get; }
        Task<int> CompleteAsync();
    }
}
