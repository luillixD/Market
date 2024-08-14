﻿using System.ComponentModel.DataAnnotations;

namespace Market.DTOs.Login
{
    public class AuthenticationDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
