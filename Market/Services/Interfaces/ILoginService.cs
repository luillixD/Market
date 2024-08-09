using Market.Models;

namespace Market.Services.Interfaces
{
    public interface ILoginService
    {
        Task<User> Register(User user);
        Task<string> Authenticate(string username, string password);
        Task<bool> ForgetPassword(string userId, string newPassword);
        Task<bool> ChangePassword(string email, string oldPassword, string newPassword);
        Task<bool> ValidateEmail(string codeValidation);
        bool IsValidEmail(string email);
        bool IsValidPassword(string password);

    }
}
