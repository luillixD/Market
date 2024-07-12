using Market.Data.Repositories.Interfaces;
using Market.Models;
using Market.Services.Interfaces;

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

            var updatedRole = await _roleRepository.Update(role);
            _logger.LogInformation($"Role updated: {updatedRole.Id}");
            return updatedRole;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _roleRepository.Delete(id);
            if (result)
            {
                _logger.LogInformation($"Role deleted: {id}");
            }
            return result;
        }
    }
}