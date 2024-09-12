using Market.DTOs.Roles;
using Market.Models;

namespace Market.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Update(int id, string fullName, string email, string phoneNumber);
        Task<User> Get(int id);
        Task<IEnumerable<User>> GetAll(int page, int pageSize);
        Task <RoleDto> GetUserRoles(int userId);
        Task Delete(int userId);
        Task<bool> Exists(int userId);
    }
}