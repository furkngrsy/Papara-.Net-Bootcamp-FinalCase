﻿using Papara_Final_Project.UnitOfWorks;
using Papara_Final_Project.DTOs;
using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            var products = await _unitOfWork.Products.GetAllProducts();
            return products.Select(p => new ProductDTO
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.IsAvailable,
                Stock = p.Stock,
                RewardRate = p.RewardRate,
                MaxReward = p.MaxReward,
                CategoryIds = p.ProductMatchCategories.Select(pc => pc.CategoryId).ToList()
            }).ToList();
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            var product = await _unitOfWork.Products.GetProductById(id);
            if (product == null)
            {
                return null;
            }

            return new ProductDTO
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                Stock = product.Stock,
                RewardRate = product.RewardRate,
                MaxReward = product.MaxReward,
                CategoryIds = product.ProductMatchCategories.Select(pc => pc.CategoryId).ToList()
            };
        }

        public async Task AddProduct(ProductDTO productDto)
        {

            var categories = await _unitOfWork.Categories.GetCategoriesByIds(productDto.CategoryIds);
            if (categories.Count != productDto.CategoryIds.Count)
            {
                throw new Exception("One or more categories do not exist.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                IsAvailable = productDto.IsAvailable,
                Stock = productDto.Stock,
                RewardRate = productDto.RewardRate,
                MaxReward = productDto.MaxReward,
                ProductMatchCategories = productDto.CategoryIds.Select(cid => new ProductMatchCategory { CategoryId = cid }).ToList()
            };

            await _unitOfWork.Products.AddProduct(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProduct(int id, ProductDTO productDto)
        {

            var product = await _unitOfWork.Products.GetProductById(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var categories = await _unitOfWork.Categories.GetCategoriesByIds(productDto.CategoryIds);
            if (categories.Count != productDto.CategoryIds.Count)
            {
                throw new Exception("One or more categories do not exist.");
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.IsAvailable = productDto.IsAvailable;
            product.Stock = productDto.Stock;
            product.RewardRate = productDto.RewardRate;
            product.MaxReward = productDto.MaxReward;

            product.ProductMatchCategories = productDto.CategoryIds.Select(cid => new ProductMatchCategory { ProductId = product.Id, CategoryId = cid }).ToList();

            await _unitOfWork.Products.UpdateProduct(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProduct(int id)
        {
            await _unitOfWork.Products.DeleteProduct(id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> IsProductAvailable(int productId, int quantity)
        {
            var product = await _unitOfWork.Products.GetProductById(productId);
            return product != null && product.Stock >= quantity && product.IsAvailable;
        }

        public async Task<decimal> GetProductPriceById(int productId)
        {
            var product = await _unitOfWork.Products.GetProductById(productId);
            return product?.Price ?? 0;
        }

        public async Task UpdateProductStock(int productId, int newStock)
        {
            var product = await _unitOfWork.Products.GetProductById(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.Stock = newStock;

            await _unitOfWork.Products.UpdateProduct(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProductAvailability(int productId, bool isAvailable)
        {
            var product = await _unitOfWork.Products.GetProductById(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.IsAvailable = isAvailable;

            await _unitOfWork.Products.UpdateProduct(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}


