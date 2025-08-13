using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Email adresi value object'i
    // Tüm modüllerde kullanılan email validation ve işlemleri
    public class Email : ValueObject
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(value));
            }
            if (!EmailRegex.IsMatch(value))
            {
                throw new ArgumentException("Invalid email format.", nameof(value));
            }
            Value = value;
        }

        // Email oluşturucu metod - validation ile
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            var normalizedEmail = email.Trim().ToLowerInvariant();
            if (!EmailRegex.IsMatch(email))
            {
                throw new ArgumentException("Invalid email format.", nameof(email));
            }
            //rfc 5321 limit
            if (normalizedEmail.Length>254)
            {
                throw new DomainException("Email adresi çok uzun", "EMAIL_TOO_LONG");
            }
            return new Email(email);
        }

        // Email domain'ini döner (örn: "gmail.com")
        public string GetDomain()
        {
            var atIndex = Value.IndexOf('@');
            if (atIndex < 0 || atIndex == Value.Length - 1)
            {
                throw new DomainException("Email formatı geçersiz.", "INVALID_EMAIL_FORMAT");
            }
            return Value.Substring(atIndex + 1);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        // Kurumsal email mi kontrol eder
        public bool IsInstitutional()
        {
            var domain = GetDomain();
            // Kurumsal email domainleri burada tanımlanabilir
            var personalDomains = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" };
            return personalDomains.Contains(domain);
        }

        public override string ToString() => Value;
        public static implicit operator string(Email email) => email.Value;

    }
}
