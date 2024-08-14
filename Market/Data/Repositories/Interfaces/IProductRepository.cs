using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> Create(Product product);
        Task<Product> Update(Product product);
        Task<Product> GetById(int id);
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetPaged(int pageNumber, int pageSize, string orderBy, decimal? price);
    }
}
