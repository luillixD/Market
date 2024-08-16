using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.DTOs.Purchase;
using Market.Models;

namespace Market.Services.Interfaces
{
    public interface IPurchaseService
    {
        public Task<bool> NewPurchase(List<int> productsId, Purchase purchase);
        public Task<TotalDto> TotalAmount(List<int> productsId);
        public Task<PurchaseDto> GetById(int id);
    }
}
