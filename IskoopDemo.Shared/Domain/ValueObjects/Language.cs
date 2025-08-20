using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class Language : ValueObject
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string NativeName { get; private set; }

        private Language() { }

        public Language(string code,string name,string nativeName)
        {
            Code = code;
            Name = name;
            NativeName = nativeName;
        }

        public static Language Create(string code)
        {
            if(string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Language code cannot be empty", nameof(code));

            code = code.Trim().ToLowerInvariant();

            if(!IsValidLanguageCode(code))
                throw new ArgumentException($"Invalid language code: {code}", nameof(code));

            try
            {
                var culture = new CultureInfo(code);
                return new Language(code, culture.DisplayName, culture.NativeName);
            }
            catch (CultureNotFoundException)
            {
                throw new ArgumentException($"Unsupported language code: {code}", nameof(code));
            }
        }

        public CultureInfo GetCultureInfo() => new(Code);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        private static bool IsValidLanguageCode(string code)
        {
            // ISO 639-1 (2 letter) or ISO 639-1 with country (en-US format)
            var pattern = @"^[a-z]{2}(-[A-Z]{2})?$";
            return Regex.IsMatch(code, pattern);
        }

        public bool IsRightToLeft()
        {
            var rtlLanguages = new[] { "ar", "he", "fa", "ur", "yi" };
            return rtlLanguages.Contains(Code.Split('-')[0]);
        }

        // Common Languages
        public static Language English => Create("en");
        public static Language Turkish => Create("tr");
        public static Language Spanish => Create("es");
        public static Language French => Create("fr");
        public static Language German => Create("de");
        public static Language Chinese => Create("zh");
        public static Language Japanese => Create("ja");
        public static Language Arabic => Create("ar");

        public static implicit operator string(Language language) => language?.Code;
        public override string ToString() => $"{Name} ({Code})";
    }
}
