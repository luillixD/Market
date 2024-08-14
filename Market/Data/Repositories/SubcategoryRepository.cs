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

        public async Task<Subcategory> Create(Subcategory subcategory)
        {
            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task<Subcategory> Update(Subcategory subcategory)
        {
            _context.Subcategories.Update(subcategory);
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task<Subcategory> GetById(int id)
        {
            return await _context.Subcategories
                                 .Include(s => s.Products)
                                 .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<IEnumerable<Subcategory>> GetAll()
        {
            return await _context.Subcategories
                                 .Include(s => s.Products)
                                 .Where(s => !s.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<bool> Exists(int subcategoryId)
        {
            return await _context.Subcategories.AnyAsync(sc => sc.Id == subcategoryId);
        }
    }
}
