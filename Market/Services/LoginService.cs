using Market.Data.Repositories.Interfaces;
using Market.Middleware;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Market.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailSender;
        private Utilities _utilities;
        private readonly Urls _url;

        public LoginService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration, ILogger<UserService> logger, IEmailService emailService, IOptions<Urls> url)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _url = url.Value;
            _utilities = new Utilities();
        }

        public async Task<User> Register(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (!_utilities.IsValidEmail(user.Email)) throw new InvalidOperationException("Invalid email format");

            if (await _userRepository.GetByEmail(user.Email) != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            user.Password = _utilities.HashPassword(user.Password);
            user.ValidationCode = _utilities.GenerateValidationCode();
            var role = await _roleRepository.GetById(user.UserRoles.FirstOrDefault().RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role does not exist");
            }

            user.UserRoles = new List<UserRole> { new UserRole { RoleId = role.Id } };

            var createdUser = await _userRepository.Create(user);
#if !DEBUG
            await _emailSender.SendEmailAsync(user.Email, "Email Validation", $"Please validate your email by clicking <a href='https://localhost:44333/api/Login/validate-email/{user.ValidationCode}'>here</a>");
#endif 
            _logger.LogInformation($"User created with role {role.Name}: {createdUser.Id}");
            return createdUser;
        }

        public async Task<string> Authenticate(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null || !VerifyPassword(password, user.Password) || !user.IsActiveUser)
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"User authenticated: {user.Id}");
            return token;
        }

        public async Task<bool> ForgetPassword(string email, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword)) return false;
            var user = await _userRepository.GetByEmail(email);
            if (user == null || !user.IsActiveUser)
            {
                return false;
            }
            user.ValidationCode = _utilities.GenerateValidationCode();
            user.IsActiveUser = false;
            user.Password = _utilities.HashPassword(newPassword);
            await _userRepository.Update(user);
#if !DEBUG
            await _emailSender.SendEmailAsync(user.Email, "Change password validation", $"Please validate your email by clicking <a href='https://localhost:44333/validate-email/{user.ValidationCode}'>here</a>");
#endif            
            return true;
        }

        public async Task<bool> ChangePassword(string userEmail, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByEmail(userEmail);
            if (user == null || !VerifyPassword(oldPassword, user.Password))
            {
                return false;
            }

            user.Password = _utilities.HashPassword(newPassword);
            await _userRepository.Update(user);
            return true;
        }

        public async Task<bool> IsVerifiedUser(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null) return false;
            return user.IsActiveUser;
        }

        public async Task<bool> ValidateEmail(string codeValidation)
        {
            var user = await _userRepository.GetByValidationCode(codeValidation);
            if (user == null) return false;

            user.IsActiveUser = true;
            user.ValidationCode = null;
            await _userRepository.Update(user);
            return true;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRoles.FirstOrDefault().Role.Name)

            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RenewToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var user = GetUserFromToken(jsonToken);

            return GenerateJwtToken(user);
        }

        private User GetUserFromToken(JwtSecurityToken token)
        {
            var userId = Convert.ToInt32(token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _userRepository.Get(userId).Result;
            if (user == null) throw new SecurityTokenException("Invalid token");
            return user;
        }

    }
}
