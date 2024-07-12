using Market.Data.Repositories.Interfaces;
using Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(int id)
        {
            var response = 0;
            var user = await Get(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                response = await _context.SaveChangesAsync();
            }
            if(response > 0) return true;
            return false;
        }

        public async Task<User> Get(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == username);
        }

        public async Task<User> GetByValidationCode(string validationCode)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ValidationCode == validationCode);
        }

        public async Task<bool> AssignRole(int userId, int roleId)
        {
            var userRole = new UserRole { UserId = userId, RoleId = roleId };
            _context.UserRoles.Add(userRole);
            var changesCount = await _context.SaveChangesAsync();
            return changesCount > 0;
        }

        public async Task<IEnumerable<string>> GetUserRoles(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAll(int page, int pageSize)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}