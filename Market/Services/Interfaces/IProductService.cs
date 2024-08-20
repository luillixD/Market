using Market.DTOs.Product;

namespace Market.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> Create(CreateProductDto productDto);
        Task<ProductDto> Update(int id, UpdateProductDto productDto);
        Task<bool> Delete(int id);
        Task<ProductDto> GetById(int id);
        Task<bool> Exists(int productId);
        Task<IEnumerable<ProductDto>> GetAll();
        Task<IEnumerable<ProductDto>> GetPaged(int pageNumber, int pageSize, string orderBy, decimal? price);
        Task<IEnumerable<ProductDto>> SearchProducts(string searchText);
        Task<IEnumerable<ProductDto>> GetBySubcategoryAsync(int subcategoryId);
    }
}
