using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Category;
using Market.Models;
using Market.Services.Interfaces;

namespace Market.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Create(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var createdCategory = await _repository.Create(category);
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task<CategoryDto> Update(int id, UpdateCategoryDto categoryDto)
        {
            var existingCategory = await _repository.GetById(id);
            if (existingCategory == null) throw new Exception("Category not found");

            _mapper.Map(categoryDto, existingCategory);
            var updatedCategory = await _repository.Update(existingCategory);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _repository.GetById(id);
            if (category == null) throw new ArgumentException("Category not found.");

            category.IsDeleted = true;
            await _repository.Update(category);
            return true;
        }

        public async Task<CategoryDto> GetById(int id)
        {
            var category = await _repository.GetById(id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAll()
        {
            var categories = await _repository.GetAll();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

    }
}
