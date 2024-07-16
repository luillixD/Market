﻿namespace Market.DTOs.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmationPassword { get; set; }
        public string Role { get; set; }
    }
}