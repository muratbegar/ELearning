using IskoopLearning.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; private set; }

        public string CountryCode { get; private set; }

        private PhoneNumber() { }

        public PhoneNumber(string value, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty", nameof(value));

            // Remove all non-digit characters except +
            var cleanValue = Regex.Replace(value, @"[^\d+]", "");

            if (string.IsNullOrWhiteSpace(cleanValue))
                throw new ArgumentException("Phone number must contain digits", nameof(value));

            if (cleanValue.Length < 7 || cleanValue.Length > 15) // ITU-T E.164 standard
                throw new ArgumentException("Phone number must be between 7 and 15 digits", nameof(value));

            Value = cleanValue;
            CountryCode = countryCode?.Trim().ToUpperInvariant();
        }

        public bool IsInternational() => Value.StartsWith("+");
        public string GetFormattedValue() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return CountryCode;
        }

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber?.Value;
        public override string ToString() => Value;
    }
}
