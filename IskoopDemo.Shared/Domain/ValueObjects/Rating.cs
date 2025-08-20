using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class Rating : ValueObject
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }

        private Rating() { } // EF Constructor

        public Rating(int value, int maxValue = 5)
        {
            if (maxValue <= 0)
                throw new ArgumentException("Max value must be positive", nameof(maxValue));

            if (value < 0 || value > maxValue)
                throw new ArgumentException($"Rating must be between 0 and {maxValue}", nameof(value));

            Value = value;
            MaxValue = maxValue;
        }

        public double GetPercentage() => (double)Value / MaxValue * 100;
        public bool IsMaxRating() => Value == MaxValue;
        public bool IsMinRating() => Value == 0;

        public static Rating FiveStar(int stars) => new(stars, 5);
        public static Rating TenPoint(int points) => new(points, 10);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return MaxValue;
        }

        public override string ToString() => $"{Value}/{MaxValue}";
    }
}
