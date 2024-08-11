using Market.DTOs.Product;

namespace Market.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> AddAsync(CreateProductDto productDto);
        Task<ProductDto> PatchAsync(int id, UpdateProductDto productDto);
        Task<bool> SoftDeleteAsync(int id);
        Task<ProductDto> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
    }
}
