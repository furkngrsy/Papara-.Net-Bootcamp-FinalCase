using Papara_Final_Project.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories();
        Task<CategoryDTO> GetCategoryById(int id);
        Task AddCategory(CategoryDTO categoryDto);
        Task UpdateCategory(int id, CategoryDTO categoryDto);
        Task DeleteCategory(int id);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(int categoryId);
    }
}
