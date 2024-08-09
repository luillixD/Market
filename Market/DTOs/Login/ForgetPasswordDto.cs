using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class ForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
