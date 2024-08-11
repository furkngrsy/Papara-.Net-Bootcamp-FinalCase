using Papara_Final_Project.Models;

namespace Papara_Final_Project.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int id);
        Task<List<Category>> GetCategoriesByIds(List<int> categoryIds);
    }
}
