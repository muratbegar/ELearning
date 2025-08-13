using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Süre value object'i
    // Kurs süreleri, ders süreleri için kullanılır
    public class Duration : ValueObject
    {

        public int TotalMinutes { get; private set; } // Süreyi dakika cinsinden tutar

        public Duration(int totalMinutes)
        {
            TotalMinutes = totalMinutes >= 0 
                ? totalMinutes 
                : throw new ArgumentException("Dakika negatif olamaz", nameof(totalMinutes));
        }

        // Dakika cinsinden süre oluşturucu
        public static Duration FromMinutes(int minutes)
        {
            if (minutes < 0)
                throw new DomainException("Süre negatif olamaz", "NEGATIVE_DURATION");
            if (minutes > 10080) // 1 hafta = 10080 dakika
                throw new DomainException("Süre 1 haftadan uzun olamaz", "DURATION_TOO_LONG");
            return new Duration(minutes);
        }

        // Saat cinsinden süre oluşturucu
        public static Duration CreateFromHours(double hours)
        {
            if (hours < 0)
                throw new DomainException("Süre negatif olamaz", "NEGATIVE_DURATION");
            if (hours > 168) // 1 hafta = 168 saat
                throw new DomainException("Süre 1 haftadan uzun olamaz", "DURATION_TOO_LONG");
            var minutes = (int)(hours * 60);
            return FromMinutes(minutes);
        }

        // Saat ve dakika ile süre oluşturucu
        public static Duration CreateFromHoursAndMinutes(int hours, int minutes)
        {
            if (hours < 0 || minutes < 0)
                throw new DomainException("Süre negatif olamaz", "NEGATIVE_DURATION");
            if (hours > 168 || (hours == 168 && minutes > 0)) // 1 hafta = 168 saat
                throw new DomainException("Süre 1 haftadan uzun olamaz", "DURATION_TOO_LONG");
            if (minutes > 60) // 1 hafta = 168 saat
                throw new DomainException("Dakika 60tan küçük olmalıdır", "DURATION_TOO_LONG");
            return FromMinutes(hours * 60 + minutes);
        }

        // Saat olarak döner
        public double TotalHours => TotalMinutes / 60.0;

        // Tam saat kısmını döner
        public int Hours => TotalMinutes / 60;

        // Kalan dakika kısmını döner
        public int Minutes => TotalMinutes % 60;

        // Formatlanmış süre gösterimi (2s 30dk)
        public string GetFormattedDuration()
        {
            if (TotalMinutes == 0) return "0 dakika";
            if (TotalMinutes < 60) return $"{TotalMinutes} dakika";

            var hours = Hours;
            var minutes = Minutes;

            if (minutes == 0) return $"{hours} saat";
            return $"{hours} saat {minutes} dakika";
        }

        // Kısa format (2:30)
        public string GetShortFormat() => $"{Hours}:{Minutes:D2}";

        // Süre toplama
        public Duration Add(Duration other) => FromMinutes(TotalMinutes + other.TotalMinutes);

        // Süre çıkarma
        public Duration Subtract(Duration other)
        {
            if (TotalMinutes < other.TotalMinutes)
                throw new DomainException("Sonuç negatif olamaz", "NEGATIVE_RESULT");

            return FromMinutes(TotalMinutes - other.TotalMinutes);
        }

        // Kısa süre mi (30 dakikadan az)
        public bool IsShort => TotalMinutes < 30;

        // Uzun süre mi (3 saattan fazla)
        public bool IsLong => TotalMinutes > 180;



        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalMinutes;
        }

        public override string ToString() => GetFormattedDuration();
    }
}
