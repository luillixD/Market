using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Review;
using Market.Models;
using Market.Services.Interfaces;

namespace Market.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository repository, IProductService productService, IUserService userService, IMapper mapper)
        {
            _repository = repository;
            _productService = productService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<ReviewDto> Create(CreateReviewDto reviewDto)
        {
            // Check if the product exists
            var productExists = await _productService.Exists(reviewDto.ProductId);
            if (!productExists) throw new ArgumentException("The specified product does not exist.");

            // Check if the user exists
            var userExists = await _userService.Exists(reviewDto.UserId);
            if (!userExists) throw new ArgumentException("The specified user does not exist.");

            var review = _mapper.Map<Review>(reviewDto);
            var createdReview = await _repository.Create(review);
            return _mapper.Map<ReviewDto>(createdReview);
        }

        public async Task<IEnumerable<ReviewDto>> GetByProductId(int productId)
        {
            var reviews = await _repository.GetByProductId(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetByUserId(int userId)
        {
            var reviews = await _repository.GetByUserId(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> Approve(int id)
        {
            var review = await _repository.GetById(id);
            if (review == null) return null;

            review.IsApproved = true;
            var updatedReview = await _repository.Update(review);
            return _mapper.Map<ReviewDto>(updatedReview);
        }
    }
}
