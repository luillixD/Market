using Market.Data.Repositories.Interfaces;
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

        // Método para obtener una compra por ID
        public async Task<Purchase> GetByIdAsync(int id)
        {
            return await _context.Purchase
                .Include(p => p.Address) // Incluye la dirección
                .Include(u => u.User) // Incluye el usuario
                .Include(p => p.PurchaseProducts) // Incluye los productos
                    .ThenInclude(pp => pp.Product) // Incluye el producto dentro de la relación
                        .ThenInclude(p => p.Subcategory) // Incluye la categoría del producto
                  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Método para obtener todas las compras
        public async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            return null;
            //return await _context.Purchase
            //    .Include(p => p.Address)
            //    .Include(p => p.PurchaseProducts)
            //        .ThenInclude(pp => pp.Product)
            //    .ToListAsync();
        }

        // Método para agregar una nueva compra
        public async Task<bool> AddAsync(Purchase purchase)
        {
            await _context.Purchase.AddAsync(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        // Método para actualizar una compra existente
        public async Task UpdateAsync(Purchase purchase)
        {
            //_context.Purchases.Update(purchase);
            //await _context.SaveChangesAsync();
        }

        // Método para eliminar una compra
        public async Task DeleteAsync(int id)
        {
            //var purchase = await _context.Purchases.FindAsync(id);
            //if (purchase != null)
            //{
            //    _context.Purchases.Remove(purchase);
            //    await _context.SaveChangesAsync();
            //}
        }

    }
}
