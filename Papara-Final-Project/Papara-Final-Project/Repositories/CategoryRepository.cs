using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesByIds(List<int> categoryIds)
    {
        return await _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToListAsync();
    }
}
