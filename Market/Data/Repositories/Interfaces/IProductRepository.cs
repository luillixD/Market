using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize, string orderBy, decimal? price);
    }
}
