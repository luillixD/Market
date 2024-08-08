using Market.Data.Repositories.Interfaces;
using Market.Middleware;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        }

        public async Task<User> Register(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if(_utilities == null) _utilities = new Utilities();

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

            await _emailSender.SendEmailAsync(user.Email, "Email Validation", $"Please validate your email by clicking <a href='https://localhost:44333/api/Login/validate-email/{user.ValidationCode}'>here</a>");

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

        public async Task<bool> ForgetPassword(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword)) return false;
            var user = await _userRepository.Get(userId);
            if (user == null)
            {
                return false;
            }
            user.ValidationCode = _utilities.GenerateValidationCode();
            user.IsActiveUser = false;
            user.Password = _utilities.HashPassword(newPassword);
            await _userRepository.Update(user);
            await _emailSender.SendEmailAsync(user.Email, "Change password validation", $"Please validate your email by clicking <a href='https://localhost:44333/validate-email/{user.ValidationCode}'>here</a>");
            
            return true;
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.Get(userId);
            if (user == null || !VerifyPassword(oldPassword, user.Password))
            {
                return false;
            }

            user.Password = _utilities.HashPassword(newPassword);
            await _userRepository.Update(user);
            return true;
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
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
    }
}
