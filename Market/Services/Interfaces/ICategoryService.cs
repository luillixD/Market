using Market.DTOs.Category;

namespace Market.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> Create(CreateCategoryDto categoryDto);
        Task<CategoryDto> Update(int id, UpdateCategoryDto categoryDto);
        Task<bool> Delete(int id);
        Task<CategoryDto> GetById(int id);
        Task<IEnumerable<CategoryDto>> GetAll();
    }
}
