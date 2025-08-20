using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class Money : ValueObject
    {

        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        public Money(decimal amount, string currency)
        {

            if(amount<0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if(string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null or empty", nameof(currency));
            if(currency.Length != 3)
                throw new ArgumentException("Currency must be a 3-letter ISO code", nameof(currency));

            Amount = Math.Round(amount,2);
            Currency = currency.ToUpperInvariant();
        }


        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        public bool IsZero() => Amount == 0;
        public bool IsPositive() => Amount > 0;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        // Common Currencies
        public static Money USD(decimal amount) => new(amount, "USD");
        public static Money EUR(decimal amount) => new(amount, "EUR");
        public static Money TRY(decimal amount) => new(amount, "TRY");

        public override string ToString() => $"{Amount:F2} {Currency}";
    }
}
