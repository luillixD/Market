using Market.Data.Repositories.Interfaces;
using Market.Models;
using Market.Services.Interfaces;
using System.Data;

namespace Market.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<Role> GetById(int id)
        {
            return await _roleRepository.GetById(id);
        }

        public async Task<Role> GetByName(string name)
        {
            return await _roleRepository.GetByName(name);
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _roleRepository.GetAll();
        }
    }
}