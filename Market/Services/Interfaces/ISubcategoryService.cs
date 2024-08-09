using Market.DTOs.Subcategory;

namespace Market.Services.Interfaces
{
    public interface ISubcategoryService
    {
        Task<SubcategoryDto> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int subcategoryId);
    }
}
