using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopLearning.Shared.Common;

namespace IskoopLearning.Shared.ValueObjects
{
    public class ProcessingAt : ValueObject
    {
        public DateTime Value { get; private set; }

        private ProcessingAt() { }


        public ProcessingAt(DateTime value)
        {
            if (value == default)
            {
                throw new ArgumentException("Created date cannot be default value.", nameof(value));
            }

            if (value > DateTime.UtcNow.AddMinutes(1))
            {
                throw new ArgumentException("Created date cannot be in the future", nameof(value));
            }

            Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);//DateTimeKind: Bu, tarih ve saatin hangi zaman dilimine (timezone) ait olduğunu belirtir.



        }

        public static ProcessingAt Now() => new(DateTime.UtcNow);
        public bool IsToday() => Value.Date == DateTime.UtcNow.Date;
        public bool IsThisWeek() => Value >= DateTime.UtcNow.AddDays(-7);
        public bool IsThisMonth() => Value.Month == DateTime.UtcNow.Month && Value.Year == DateTime.UtcNow.Year;

        public TimeSpan AgeFromNow() => DateTime.UtcNow - Value; //Bu metot, CreatedDate'in oluşturulma tarihinden şu ana kadar geçen süreyi TimeSpan olarak döndürür.

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator DateTime(ProcessingAt createdDate) => createdDate?.Value ?? default; //DateTime'a dönüştürme: CreatedDate nesnesini DateTime'a dönüştürmek için bir dönüşüm operatörü eklenmiş. CreatedDate nesnesi bir DateTime'a kolayca dönüştürülebilir.
        public static implicit operator ProcessingAt(DateTime dateTime) => new(dateTime); //DateTime'dan CreatedDate'e dönüşüm: Benzer şekilde, DateTime nesnesi doğrudan CreatedDate nesnesine dönüştürülebilir.

        public override string ToString() => Value.ToString("yyyy-MM-ddTHH:mm:ssZ"); //CreatedDate'in tarih değerini ISO 8601 formatında (yyyy-MM-ddTHH:mm:ssZ) döndürür.
    }
}
