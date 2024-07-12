using Market.Models;

namespace Market.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role> GetById(int id);
        Task<Role> GetByName(string name);
        Task<IEnumerable<Role>> GetAll();
        Task<Role> Create(Role role);
        Task<Role> Update(Role role);
        Task<bool> Delete(int id);
    }
}