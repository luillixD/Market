using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public string ValidationCode { get; set; }

        public bool IsActiveUser { get; set; } = false;

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
