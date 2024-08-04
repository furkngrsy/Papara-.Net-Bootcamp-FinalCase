public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProducts();
    Task<ProductDTO> GetProductById(int id);
    Task AddProduct(ProductDTO productDto);
    Task UpdateProduct(int id, ProductDTO productDto);
    Task DeleteProduct(int id);
}
