using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string ToTitleCase(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string ToPascalCase(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return Regex.Replace(value, @"(?:^|_)([a-z])", match => match.Groups[1].Value.ToUpper())
                .Replace("_", "");
        }

        public static string ToCamelCase(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            var pascalCase = value.ToPascalCase();
            return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
        }

        public static string ToKebabCase(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return Regex.Replace(value, @"(?<!^)([A-Z])", "-$1").ToLower();
        }

        public static string ToSnakeCase(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return Regex.Replace(value, @"(?<!^)([A-Z])", "_$1").ToLower();
        }


        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (value.IsNullOrWhiteSpace() || value.Length <= maxLength)
                return value;

            return value.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public static string RemoveWhitespace(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return Regex.Replace(value, @"\s+", "");
        }

        public static string NormalizeWhitespace(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            return Regex.Replace(value.Trim(), @"\s+", " ");
        }

        public static string Mask(this string value, int visibleChars = 4, char maskChar = '*')
        {
            if (value.IsNullOrWhiteSpace() || value.Length <= visibleChars)
                return value;

            var visiblePart = value.Substring(value.Length - visibleChars);
            var maskedPart = new string(maskChar, value.Length - visibleChars);

            return maskedPart + visiblePart;
        }

        public static string ToSlug(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return value;

            value = value.ToLowerInvariant();
            value = Regex.Replace(value, @"[^a-z0-9\s-]", "");
            value = Regex.Replace(value, @"\s+", "-");
            value = Regex.Replace(value, @"-+", "-");

            return value.Trim('-');
        }

        public static bool IsValidEmail(this string email)
        {
            if (email.IsNullOrWhiteSpace()) return false;

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

        public static bool IsValidUrl(this string url)
        {
            if (url.IsNullOrWhiteSpace()) return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        public static string ToBase64(this string value)
        {
            if (value.IsNullOrEmpty()) return value;

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string value)
        {
            if (value.IsNullOrEmpty()) return value;

            try
            {
                var bytes = Convert.FromBase64String(value);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return value;
            }
        }

        public static string GenerateHash(this string value, string salt = null)
        {
            if (value.IsNullOrEmpty()) return value;

            using var sha256 = SHA256.Create();
            var saltedValue = value + (salt ?? "");
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedValue));

            return Convert.ToBase64String(hash);
        }

        public static string[] SplitCsv(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return Array.Empty<string>();

            return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim())
                       .ToArray();
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            return source?.IndexOf(value, comparison) >= 0;
        }

        public static string ReplaceIgnoreCase(this string source, string oldValue, string newValue)
        {
            if (source.IsNullOrEmpty()) return source;

            return Regex.Replace(source, Regex.Escape(oldValue), newValue, RegexOptions.IgnoreCase);
        }

        public static int WordCount(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return 0;

            return value.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string Reverse(this string value)
        {
            if (value.IsNullOrEmpty()) return value;

            var charArray = value.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
