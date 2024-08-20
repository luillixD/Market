using Market.Data.Repositories.Interfaces;
using Market.DTOs.Purchase;
using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Purchase> GetByIdAsync(int id)
        {
            return await _context.Purchase
                .Include(p => p.Address) 
                .Include(u => u.User) 
                .Include(p => p.PurchaseProducts) 
                    .ThenInclude(pp => pp.Product) 
                        .ThenInclude(p => p.Subcategory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Purchase>> GetPurchasesResumeAsync(int userId, int page, int pageSize)
        {
            return await _context.Purchase
                .Include(p => p.PurchaseProducts)
                 .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(Purchase purchase)
        {
            await _context.Purchase.AddAsync(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Purchase purchase)
        {
            _context.Purchase.Update(purchase);
            var result = await _context.SaveChangesAsync();
            return true;
        }
    }
}
