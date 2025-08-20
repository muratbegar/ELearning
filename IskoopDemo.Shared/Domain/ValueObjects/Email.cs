using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }

        private Email() { }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email cannot be empty.", nameof(value));
            }
            value = value.Trim().ToLowerInvariant();
            if (!IsValidEmail(value))
            {
                throw new ArgumentException("Invalid email format.", nameof(value));
            }
            if (value.Length > 254) // RFC 5321 limit
                throw new ArgumentException("Email cannot exceed 254 characters", nameof(value));

            Value = value;
        }
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string GetDomain() => Value.Split('@')[1];
        public string GetLocalPart() => Value.Split('@')[0];
        public bool IsFromDomain(string domain) => GetDomain().Equals(domain, StringComparison.OrdinalIgnoreCase);


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email email) => email?.Value;
        public static implicit operator Email(string email) => new(email);

        public override string ToString() => Value;
    }
}
