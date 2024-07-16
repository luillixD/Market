using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class ChangePassworDto
    {
        [EmailAddress]
        public string email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
