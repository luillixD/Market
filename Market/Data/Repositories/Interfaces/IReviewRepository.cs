using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> Create(Review review);
        Task<Review> Update(Review review);
        Task<Review> GetById(int id);
        Task<IEnumerable<Review>> GetByProductId(int productId);
        Task<IEnumerable<Review>> GetByUserId(int userId);
    }
}
