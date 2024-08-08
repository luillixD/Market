namespace Market.DTOs.Login
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmationPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
