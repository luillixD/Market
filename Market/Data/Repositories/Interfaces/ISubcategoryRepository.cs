using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<Subcategory> Create(Subcategory subcategory);
        Task<Subcategory> Update(Subcategory subcategory);
        Task<Subcategory> GetById(int id);
        Task<IEnumerable<Subcategory>> GetAll();
        Task<bool> Exists(int subcategoryId);
    }
}
