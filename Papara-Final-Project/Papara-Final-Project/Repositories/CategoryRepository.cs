using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;

namespace Papara_Final_Project.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task AddCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
        }

        public async Task<List<Category>> GetCategoriesByIds(List<int> categoryIds)
        {
            return await _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();
        }
    }
}
