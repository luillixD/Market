using Market.Models;

namespace Market.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role> GetById(int id);
        Task<Role> GetByName(string name);
        Task<IEnumerable<Role>> GetAll();
    }
}