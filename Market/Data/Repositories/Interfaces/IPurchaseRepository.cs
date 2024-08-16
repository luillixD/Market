using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<Purchase> GetByIdAsync(int id);
        Task<IEnumerable<Purchase>> GetAllAsync();
        Task<bool> AddAsync(Purchase purchase);
        Task UpdateAsync(Purchase purchase);
        Task DeleteAsync(int id);
    }
}
