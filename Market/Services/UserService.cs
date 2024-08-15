using Market.Data.Repositories.Interfaces;
using Market.DTOs.Roles;
using Market.Middleware;
using Market.Models;
using Market.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
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
            _utilities = new Utilities();
        }

        public async Task<User> Update(int id, string fullName, string email, string phoneNumber)
        {
            var user = await _userRepository.Get(id);
            if(!string.IsNullOrEmpty(fullName)) user.FullName = fullName;
            if(!string.IsNullOrEmpty(email)) user.Email = email;
            if(!string.IsNullOrEmpty(phoneNumber)) user.PhoneNumber = phoneNumber;
            if (!_utilities.IsValidEmail(user.Email)) throw new InvalidOperationException("Invalid email format");
            return await _userRepository.Update(user);
        }

        public async Task<User> Get(int id)
        {
            return await _userRepository.Get(id);
        }

        public async Task<IEnumerable<User>> GetAll(int page, int pageSize)
        {
            return await _userRepository.GetAll(page, pageSize);
        }

        public async Task<RoleDto> GetUserRoles(int userId)
        {
            return await _userRepository.GetUserRoles(userId);
        }

        public async Task Delete(int userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null) throw new InvalidOperationException("User not found");
            user.IsActiveUser = false;
            await _userRepository.Delete(user);
        }

        public async Task<bool> Exists(int userId)
        {
            return await _userRepository.Exists(userId);
        }
    }
}