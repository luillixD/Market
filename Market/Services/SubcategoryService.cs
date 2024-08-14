using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Subcategory;
using Market.Models;
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

        public async Task<SubcategoryDto> Create(CreateSubcategoryDto subcategoryDto)
        {
            var subcategory = _mapper.Map<Subcategory>(subcategoryDto);
            var createdSubcategory = await _repository.Create(subcategory);
            return _mapper.Map<SubcategoryDto>(createdSubcategory);
        }

        public async Task<SubcategoryDto> Update(int id, UpdateSubcategoryDto subcategoryDto)
        {
            // Check if the subcategory exists
            var existingSubcategory = await _repository.GetById(id);
            if (existingSubcategory == null) throw new Exception("Subcategory not found");

            // Check if the associated category exists
            // I MUST IMPLEMENT IT

            _mapper.Map(subcategoryDto, existingSubcategory);
            var updatedSubcategory = await _repository.Update(existingSubcategory);
            return _mapper.Map<SubcategoryDto>(updatedSubcategory);
        }

        public async Task<bool> Delete(int id)
        {
            var subcategory = await _repository.GetById(id);
            if (subcategory == null) throw new ArgumentException("Subcategory not found.");

            subcategory.IsDeleted = true;
            await _repository.Update(subcategory);
            return true;
        }

        public async Task<SubcategoryDto> GetById(int id)
        {
            var subcategory = await _repository.GetById(id);
            return _mapper.Map<SubcategoryDto>(subcategory);
        }

        public async Task<IEnumerable<SubcategoryDto>> GetAll()
        {
            var subcategories = await _repository.GetAll();
            return _mapper.Map<IEnumerable<SubcategoryDto>>(subcategories);
        }

        public async Task<bool> ExistsAsync(int subcategoryId)
        {
            return await _repository.Exists(subcategoryId);
        }
    }
}
