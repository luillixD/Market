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

        public async Task<Role> Create(Role role)
        {
            if (await _roleRepository.GetByName(role.Name) != null)
            {
                throw new InvalidOperationException($"Role with name '{role.Name}' already exists");
            }

            var createdRole = await _roleRepository.Create(role);
            _logger.LogInformation($"Role created: {createdRole.Id}");
            return createdRole;
        }

        public async Task<Role> Update(Role role)
        {
            var existingRole = await _roleRepository.GetById(role.Id);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with id '{role.Id}' does not exist");
            }
            existingRole.Name = role.Name;

            var updatedRole = await _roleRepository.Update(existingRole);
            _logger.LogInformation($"Role updated: {updatedRole.Id}");
            return updatedRole;
        }

        public async Task Delete(int id)
        {
            var existingRole = await _roleRepository.GetById(id);
            if (existingRole == null)
            {
                throw new InvalidOperationException($"Role with id '{id}' does not exist");
            }
            existingRole.IsActiveRole = false;
            await _roleRepository.Delete(existingRole);
        }
    }
}