using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
