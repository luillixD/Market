using Market.DTOs.Review;

namespace Market.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> Create(CreateReviewDto reviewDto);
        Task<IEnumerable<ReviewDto>> GetByProductId(int productId);
        Task<IEnumerable<ReviewDto>> GetByUserId(int userId);
        Task<ReviewDto> Approve(int id);
    }
}
