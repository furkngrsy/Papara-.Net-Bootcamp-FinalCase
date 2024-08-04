using Papara_Final_Project.Models;

namespace Papara_Final_Project.Services
{
    public interface IProductService
    {
        Product GetProductById(int id);
        IEnumerable<Product> GetAllProducts();
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
    }
}
