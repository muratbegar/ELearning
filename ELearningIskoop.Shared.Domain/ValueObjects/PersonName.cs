using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Kişi ismi value object'i
    // Öğrenci ve eğitmen isimleri için kullanılır
    public class PersonName : ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private PersonName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        // PersonName oluşturucu metod - validation ile
        public static PersonName Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("İsim boş geçilemez", "FIRST_NAME_REQUIRED");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Soyad boş geçilemez", "LAST_NAME_REQUIRED");


            var cleanFirstName = firstName.Trim();
            var cleanLastName = lastName.Trim();

            if (cleanFirstName.Length < 2 || cleanFirstName.Length > 50)
                throw new DomainException("İsim 2-50 karakter arasında olmalıdır", "FIRST_NAME_LENGTH_INVALID");
            if(cleanLastName.Length <2 || cleanLastName.Length>50)
                throw new DomainException("Soyad 2-50 karakter arasında olmalıdır", "LAST_NAME_LENGTH_INVALID");

            // Türkçe karakter desteği ile capitalize
            return new PersonName(
                CapitalizeTurkish(cleanFirstName),
                CapitalizeTurkish(cleanLastName));

        }

        // Tam isim döner
        public string FullName => $"{FirstName} {LastName}";

        // isim baş haflerini döner
        public string Initials => $"{FirstName[0].ToString().ToUpper()}.{LastName[0].ToString().ToUpper()}";

        // Türkçe karakterleri destekleyen capitalize
        private static string CapitalizeTurkish(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            // Türkçe karakterler için özel durumlar
            var turkishChars = new Dictionary<char, char>
            {
                {'i', 'İ'},
                {'ı', 'I'},
                {'ş', 'Ş'},
                {'ğ', 'Ğ'},
                {'ü', 'Ü'},
                {'ö', 'Ö'},
                {'ç', 'Ç'}
            };
            var result = new StringBuilder();
            foreach (var c in input)
            {
                if (turkishChars.ContainsKey(c))
                    result.Append(turkishChars[c]);
                else
                    result.Append(char.ToUpper(c));
            }
            return result.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
        public override string ToString() => FullName;
    }
}
