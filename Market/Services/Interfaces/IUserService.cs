using Market.Models;

namespace Market.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Create(User user, string role);
        Task<User> Update(User user);
        Task<bool> Delete(int id);
        Task<User> Get(int id);
        Task<IEnumerable<User>> GetAll(int page, int pageSize);
        Task<bool> ValidateEmail(string codeValidation);
        Task<string> Authenticate(string username, string password);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        Task<bool> Exists(int id);
        Task<IEnumerable<string>> GetUserRoles(int userId);
    }
}