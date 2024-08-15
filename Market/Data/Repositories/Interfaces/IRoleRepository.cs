using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetById(int id);
        Task<Role> GetByName(string name);
        Task<IEnumerable<Role>> GetAll();
    }
}