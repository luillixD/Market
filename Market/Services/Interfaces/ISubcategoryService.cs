using Market.DTOs.Subcategory;

namespace Market.Services.Interfaces
{
    public interface ISubcategoryService
    {
        Task<SubcategoryDto> Create(CreateSubcategoryDto subcategoryDto);
        Task<SubcategoryDto> Update(int id, UpdateSubcategoryDto subcategoryDto);
        Task<bool> Delete(int id);
        Task<SubcategoryDto> GetById(int id);
        Task<IEnumerable<SubcategoryDto>> GetAll();
        Task<bool> ExistsAsync(int subcategoryId);
    }
}
