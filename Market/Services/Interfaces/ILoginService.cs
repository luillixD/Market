using Market.Models;

namespace Market.Services.Interfaces
{
    public interface ILoginService
    {
        Task<User> Register(User user);
        Task<string> Authenticate(string username, string password);
        Task<bool> ForgetPassword(int userId, string newPassword);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        Task<bool> ValidateEmail(string codeValidation);
    }
}
