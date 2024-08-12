using Market.DTOs.Roles;
using Market.Models;

namespace Market.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User> Update(User user);
        Task Delete(User user);
        Task<User> Get(int id);
        Task<User> GetByEmail(string email);
        Task<IEnumerable<User>> GetAll(int page, int pageSize);
        Task<bool> Exists(int id);
        Task<User> GetByValidationCode(string validationCode);
        Task<bool> AssignRole(int userId, int roleId);
        Task<RoleDto> GetUserRoles(int userId);
    }
}