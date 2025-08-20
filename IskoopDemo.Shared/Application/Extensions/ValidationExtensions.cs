using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class ValidationExtensions
    {
        public static ValidationResult IsRequired(this object value, string fieldName)
        {
            if (value == null || value is string str && str.IsNullOrWhiteSpace())
            {
                return new ValidationResult($"The {fieldName} field is required.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult HasMinLength(this string value, int minLength, string fieldName)
        {
            if (value != null && value.Length < minLength)
            {
                return new ValidationResult($"The {fieldName} field must be at least {minLength} characters long.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult HasMaxLength(this string value, int maxLength, string fieldName)
        {
            if (value != null && value.Length > maxLength)
            {
                return new ValidationResult($"The {fieldName} field cannot exceed {maxLength} characters.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsInRange<T>(this T value, T min, T max, string fieldName) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                return new ValidationResult($"The {fieldName} field must be between {min} and {max}.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsValidEmail(this string email, string fieldName = "Email")
        {
            if (!email.IsNullOrWhiteSpace() && !email.IsValidEmail())
            {
                return new ValidationResult($"The {fieldName} field is not a valid email address.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsValidUrl(this string url, string fieldName = "URL")
        {
            if (!url.IsNullOrWhiteSpace() && !url.IsValidUrl())
            {
                return new ValidationResult($"The {fieldName} field is not a valid URL.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult MatchesPattern(this string value, string pattern, string fieldName, string errorMessage = null)
        {
            if (!value.IsNullOrWhiteSpace() && !Regex.IsMatch(value, pattern))
            {
                return new ValidationResult(errorMessage ?? $"The {fieldName} field format is invalid.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsDateInFuture(this DateTime date, string fieldName)
        {
            if (date <= DateTime.Now)
            {
                return new ValidationResult($"The {fieldName} field must be a future date.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsDateInPast(this DateTime date, string fieldName)
        {
            if (date >= DateTime.Now)
            {
                return new ValidationResult($"The {fieldName} field must be a past date.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsPositive(this decimal value, string fieldName)
        {
            if (value <= 0)
            {
                return new ValidationResult($"The {fieldName} field must be a positive number.");
            }
            return ValidationResult.Success;
        }

        public static ValidationResult IsPhoneNumber(this string phoneNumber, string fieldName = "Phone Number")
        {
            if (!phoneNumber.IsNullOrWhiteSpace())
            {
                var pattern = @"^[\+]?[1-9][\d]{0,15}$";
                if (!Regex.IsMatch(phoneNumber.RemoveWhitespace(), pattern))
                {
                    return new ValidationResult($"The {fieldName} field is not a valid phone number.");
                }
            }
            return ValidationResult.Success;
        }

        public static IEnumerable<ValidationResult> ValidateObject(this object obj)
        {
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(obj, context, results, true);

            return results;
        }

        public static bool IsValid(this object obj)
        {
            return !obj.ValidateObject().Any();
        }
    }
}
