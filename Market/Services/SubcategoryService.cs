using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Subcategory;
using Market.Services.Interfaces;

namespace Market.Services
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly ISubcategoryRepository _repository;
        private readonly IMapper _mapper;

        public SubcategoryService(ISubcategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SubcategoryDto> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return _mapper.Map<SubcategoryDto>(product);
        }
        public async Task<bool> ExistsAsync(int subcategoryId)
        {
            return await _repository.ExistsAsync(subcategoryId);
        }
    }
}
