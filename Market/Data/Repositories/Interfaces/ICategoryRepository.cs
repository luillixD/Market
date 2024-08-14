using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> Create(Category category);
        Task<Category> Update(Category category);
        Task<Category> GetById(int id);
        Task<IEnumerable<Category>> GetAll();
    }
}
