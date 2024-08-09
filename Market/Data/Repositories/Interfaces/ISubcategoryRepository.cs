using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<Subcategory> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int subcategoryId);
    }
}
