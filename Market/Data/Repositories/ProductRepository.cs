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

        public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize, string orderBy = null, decimal? price = null)
        {
            var query = _context.Products.Where(p => !p.IsDeleted);

            // Filtering by price
            if (price.HasValue)
            {
                query = query.Where(p => p.Price <= price.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id); // Default sorting by ID
                        break;
                }
            }

            return await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

    }
}