using Market.Data.Repositories.Interfaces;
using Market.Middleware;
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
        private Utilities _utilities;

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
            if (_utilities == null) _utilities = new Utilities();

            user.Password = HashPassword(user.Password);
            user.ValidationCode = _utilities.GenerateValidationCode();

            var role = await _roleRepository.GetByName(roleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' does not exist");
            }

            user.UserRoles = new List<UserRole> { new UserRole { RoleId = role.Id } };

            var createdUser = await _userRepository.Create(user);

            SendValidationEmail(createdUser);

            _logger.LogInformation($"User created with role {roleName}: {createdUser.Id}");
            return createdUser;
        }

        internal void SendValidationEmail(User user)
        {
            // Send email with validation code

            
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
    }
}