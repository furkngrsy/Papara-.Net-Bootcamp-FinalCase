using Papara_Final_Project.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Papara_Final_Project.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProducts();
        Task<ProductDTO> GetProductById(int id);
        Task AddProduct(ProductDTO productDto);
        Task UpdateProduct(int id, ProductDTO productDto);
        Task DeleteProduct(int id);
        Task<bool> IsProductAvailable(int productId, int quantity);
        Task<decimal> GetProductPriceById(int productId);
        Task UpdateProductStock(int productId, int newStock); 
        Task UpdateProductAvailability(int productId, bool isAvailable); 
    }
}
