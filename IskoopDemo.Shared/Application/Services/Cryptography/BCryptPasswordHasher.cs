using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces.Cryptography;
using IskoopDemo.Shared.Models.Cryptography;
using Org.BouncyCastle.Crypto.Generators;

namespace IskoopDemo.Shared.Application.Services.Cryptography
{
    public class BCryptPasswordHasher : IPasswordHasher
    {

        private const int WorkFactor = 12; // Adjust work factor as needed for security/performance balance

        

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);


        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch 
            {
                return false;
            }
        }

        public PasswordStrength CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return PasswordStrength.VeryWeak;

            int score = 0;
            // Length
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Length >= 16) score++;

            // Complexity
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"\d")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]")) score++;


            // Common patterns (negative score)
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"(.)\1{2,}")) score--;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"(012|123|234|345|456|567|678|789|890|abc|bcd|cde|def)")) score--;


            return score switch
            {
                >= 7 => PasswordStrength.VeryStrong,
                >= 5 => PasswordStrength.Strong,
                >= 3 => PasswordStrength.Medium,
                >= 1 => PasswordStrength.Weak,
                _ => PasswordStrength.VeryWeak
            };


        }
    }
}
