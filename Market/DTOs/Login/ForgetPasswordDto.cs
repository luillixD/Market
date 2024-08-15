using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class ForgetPasswordDto
    {
        [Required(ErrorMessage = "Name is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "The password is not valid, the password needs a minimum of 8 characters in length, a special character and a number.")]
        public string NewPassword { get; set; }
    }
}
