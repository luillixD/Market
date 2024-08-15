using Market.Data.Repositories.Interfaces;
using Market.Models;
using Microsoft.EntityFrameworkCore;

namespace Market.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> Create(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> Update(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> GetById(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<IEnumerable<Review>> GetByProductId(int productId)
        {
            return await _context.Reviews
                                 .Where(r => r.ProductId == productId && r.IsApproved)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserId(int userId)
        {
            return await _context.Reviews
                                 .Where(r => r.UserId == userId && r.IsApproved)
                                 .ToListAsync();
        }

    }
}
