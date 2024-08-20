using Market.DTOs.Bill;
using Market.DTOs.Product;
using Market.DTOs.Purchase;
using Market.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlX.XDevAPI.Common;

namespace Market.Services.Interfaces
{
    public interface IPurchaseService
    {
        public Task<bool> NewPurchase(List<int> productsId, Purchase purchase);
        public Task<TotalDto> TotalAmount(List<int> productsId);
        public Task<PurchaseDto> GetById(int id);
        public Task<PurchaseDto> GetByIdAndUser(int id, int userId);
        public Task<bool> RejectPurchase(int id);
        public Task<bool> AcceptPurchase(int id);
        public Task<bool> DonePurchase(int id);
        public Task<List<PurchaseResumeDto>> PurchaseResume(int userId, int page, int pageSize);
        public Task<bool> UpdatePurchase(List<int> productsId, int purchaseIde);
    }
}
