using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Domain;

namespace ELearningIskoop.Shared.Domain.ValueObjects
{
    // Para birimi value object'i
    // Kurs fiyatları, ödemeler için kullanılır
    public class Money : ValueObject
    {

        public decimal Amount { get; private set; }

        public string Currency { get; private set; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));
            Amount = amount;
            Currency = currency;
        }

        // Money oluşturucu metod - validation ile
        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
                throw new DomainException("Tutar negatif olamaz", "NEGATIVE_AMOUNT");

            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Para birimi belirtilmelidir", "CURRENCY_REQUIRED");

            var normalizedCurrency = currency.Trim().ToUpperInvariant();

            // Desteklenen para birimleri
            var supportedCurrencies = new[] { "TRY", "USD", "EUR", "GBP" };
            if (!supportedCurrencies.Contains(normalizedCurrency))
                throw new DomainException($"Desteklenmeyen para birimi: {normalizedCurrency}", "UNSUPPORTED_CURRENCY");

            return new Money(amount, normalizedCurrency);
        }


        //Türk Lirası olarak oluşturma
        public static Money CreateTRY(decimal amount) => Create(amount, "TRY");

        // ABD Doları oluşturucu
        public static Money CreateUSD(decimal amount) => Create(amount, "USD");

        //euro oluşturucu
        public static Money CreateEUR(decimal amount) => Create(amount, "EUR");

        //para birimi sembolü döner
        public string GetCurrencySymbol()
        {
            return Currency switch
            {
                "TRY" => "₺",
                "USD" => "$",
                "EUR" => "€",
                "GBP" => "£",
                _ => throw new DomainException($"Desteklenmeyen para birimi: {Currency}", "UNSUPPORTED_CURRENCY")
            };
        }

        // Formatlanmış para gösterimi
        public string GetFormattedAmount() => $"{Amount:N2} {GetCurrencySymbol()}";

        // Para toplama (aynı para birimi)
        public Money Add(Money other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (Currency != other.Currency)
                throw new DomainException("Para birimleri eşleşmiyor", "CURRENCY_MISMATCH");
            return new Money(Amount + other.Amount, Currency);
        }

        // Para çıkarma (aynı para birimi)
        public Money Subtract(Money other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (Currency != other.Currency)
                throw new DomainException("Para birimleri eşleşmiyor", "CURRENCY_MISMATCH");
            if (Amount < other.Amount)
                throw new DomainException("Yetersiz bakiye", "INSUFFICIENT_BALANCE");
            return new Money(Amount - other.Amount, Currency);
        }

        // İndirim uygula (yüzde)
        public Money ApplyDiscount(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new DomainException("İndirim yüzdesi 0-100 arasında olmalıdır", "INVALID_DISCOUNT_PERCENTAGE");
            var discountAmount = Amount * (percentage / 100);
            return new Money(Amount - discountAmount, Currency);
        }

        // Ücretsiz mi kontrol eder
        public bool IsFree() => Amount == 0;


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{GetFormattedAmount()} ({Currency})";
        }
    }
}
