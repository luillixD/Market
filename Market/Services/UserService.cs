using Market.Data.Repositories.Interfaces;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Market.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> Create(User user, string roleName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Role name cannot be empty", nameof(roleName));

            if (await _userRepository.GetByEmail(user.Email) != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            user.Password = HashPassword(user.Password);
            user.ValidationCode = GenerateValidationCode();

            var role = await _roleRepository.GetByName(roleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' does not exist");
            }

            user.UserRoles = new List<UserRole> { new UserRole { RoleId = role.Id } };

            var createdUser = await _userRepository.Create(user);
            _logger.LogInformation($"User created with role {roleName}: {createdUser.Id}");
            return createdUser;
        }

        public async Task<User> Update(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userRepository.Update(user);
        }

        public async Task<bool> Delete(int id)
        {
            return await _userRepository.Delete(id);
        }

        public async Task<User> Get(int id)
        {
            return await _userRepository.Get(id);
        }

        public async Task<IEnumerable<User>> GetAll(int page, int pageSize)
        {
            return await _userRepository.GetAll(page, pageSize);
        }

        public async Task<bool> ValidateEmail(string codeValidation)
        {
            var user = await _userRepository.GetByValidationCode(codeValidation);
            if (user == null) return false;

            user.IsEmailValidated = true;
            user.ValidationCode = null;
            await _userRepository.Update(user);
            return true;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await _userRepository.GetByEmail(username);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"User authenticated: {user.Id}");
            return token;
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.Get(userId);
            if (user == null || !VerifyPassword(oldPassword, user.Password))
            {
                return false;
            }

            user.Password = HashPassword(newPassword);
            await _userRepository.Update(user);
            return true;
        }

        public async Task<bool> Exists(int id)
        {
            return await _userRepository.Exists(id);
        }

        public async Task<IEnumerable<string>> GetUserRoles(int userId)
        {
            return await _userRepository.GetUserRoles(userId);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateValidationCode()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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