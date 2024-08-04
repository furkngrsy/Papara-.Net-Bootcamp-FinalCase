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
            Products = new ProductRepository(_context);
            Users = new UserRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public IProductRepository Products { get; private set; }
        public IUserRepository Users { get; private set; }
        public ICategoryRepository Categories { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
