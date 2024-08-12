using Market.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace Market.Middleware
{
    public class Utilities
    {
        internal string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        internal string GenerateValidationCode()
        {
            return Guid.NewGuid().ToString();
        }

        public bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }
    }
}
