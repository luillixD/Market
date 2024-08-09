using Market.Data.Repositories.Interfaces;
using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.Data.Repositories
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public SubcategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Subcategory> GetByIdAsync(int id)
        {
            return await _context.Subcategories.FindAsync(id);
        }
        public async Task<bool> ExistsAsync(int subcategoryId)
        {
            return await _context.Subcategories.AnyAsync(sc => sc.Id == subcategoryId);
        }
    }
}
