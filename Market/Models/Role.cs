using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // Relación muchos a muchos con User
        public ICollection<UserRole> UserRoles { get; set; }
    }
}