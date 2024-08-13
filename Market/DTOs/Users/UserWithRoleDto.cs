using Market.DTOs.Roles;

namespace Market.DTOs.Users
{
    public class UserWithRoleDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public RoleDto Role { get; set; }
    }
}
