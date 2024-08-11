﻿using Microsoft.EntityFrameworkCore;
using Market.Data.Repositories.Interfaces;
using Market.Models;

namespace Market.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                                 .Where(p => p.Id == id && !p.IsDeleted)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                                 .Where(p => !p.IsDeleted)
                                 .ToListAsync();
        }
    }
}
