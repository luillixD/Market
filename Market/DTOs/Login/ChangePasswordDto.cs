using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "The password is not valid, the password needs a minimum of 8 characters in length, a special character and a number.")]
        public string NewPassword { get; set; }
    }
}
