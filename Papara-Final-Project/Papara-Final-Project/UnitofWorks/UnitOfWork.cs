using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;
using Papara_Final_Project.Repositories;

namespace Papara_Final_Project.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(context);
            Products = new ProductRepository(context);
            Users = new UserRepository(context);
            Coupons = new CouponRepository(context); 
        }

        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IUserRepository Users { get; private set; }
        public ICouponRepository Coupons { get; private set; } 
        public DbSet<ProductMatchCategory> ProductMatchCategories => _context.ProductMatchCategories;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
