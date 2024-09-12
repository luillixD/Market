using Market.DTOs.Purchase;
using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<Purchase> GetByIdAsync(int id);
        Task<bool> AddAsync(Purchase purchase);
        Task<bool> UpdateAsync(Purchase purchase);
        Task<List<Purchase>> GetPurchasesResumeAsync(int userId, int page, int pageSize);
    }
}
