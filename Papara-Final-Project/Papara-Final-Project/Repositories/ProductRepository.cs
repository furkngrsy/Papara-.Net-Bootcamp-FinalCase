using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;


namespace Papara_Final_Project.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.ProductMatchCategories)
                .ThenInclude(pc => pc.Category)
                .ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.ProductMatchCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public async Task<List<Product>> GetProductsByCategoryId(int categoryId)
        {
            return await _context.ProductMatchCategories
                .Where(pc => pc.CategoryId == categoryId)
                .Include(pc => pc.Product)
                .Select(pc => pc.Product)
                .ToListAsync();
        }
    }
}


