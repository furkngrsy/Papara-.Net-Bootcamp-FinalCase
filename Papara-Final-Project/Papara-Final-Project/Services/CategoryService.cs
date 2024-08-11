using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;
using Papara_Final_Project.UnitOfWorks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetAllCategories();
            return categories.Select(c => new CategoryDTO
            {
                Name = c.Name,
                Url = c.Url,
                Tag = c.Tag
            }).ToList();
        }

        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryById(id);
            if (category == null)
            {
                return null;
            }

            return new CategoryDTO
            {
                Name = category.Name,
                Url = category.Url,
                Tag = category.Tag
            };
        }

        public async Task AddCategory(CategoryDTO categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Url = categoryDto.Url,
                Tag = categoryDto.Tag
            };

            await _unitOfWork.Categories.AddCategory(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCategory(int id, CategoryDTO categoryDto)
        {

            var category = await _unitOfWork.Categories.GetCategoryById(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            category.Name = categoryDto.Name;
            category.Url = categoryDto.Url;
            category.Tag = categoryDto.Tag;

            await _unitOfWork.Categories.UpdateCategory(category);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteCategory(int id)
        {
            var products = await _unitOfWork.ProductMatchCategories
                .Where(pc => pc.CategoryId == id)
                .Include(pc => pc.Product)
                .ToListAsync();

            if (products.Any())
            {
                throw new ValidationException("Cannot delete category with associated products.");
            }

            await _unitOfWork.Categories.DeleteCategory(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(int categoryId)
        {
            var productMatches = await _unitOfWork.ProductMatchCategories
                .Where(pc => pc.CategoryId == categoryId)
                .Include(pc => pc.Product)
                .ToListAsync();

            if (!productMatches.Any())
            {
                return Enumerable.Empty<ProductDTO>();
            }

            return productMatches.Select(pm => new ProductDTO
            {
                Name = pm.Product.Name,
                Description = pm.Product.Description,
                Price = pm.Product.Price,
                IsAvailable = pm.Product.IsAvailable,
                Stock = pm.Product.Stock,
                RewardRate = pm.Product.RewardRate,
                MaxReward = pm.Product.MaxReward,
                CategoryIds = pm.Product.ProductMatchCategories.Select(pc => pc.CategoryId).ToList()
            }).ToList();
        }
    }
}
