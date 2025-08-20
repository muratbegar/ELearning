using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopLearning.Shared.Common;

namespace IskoopLearning.Shared.ValueObjects
{
    public class Duration : ValueObject
    {
        public TimeSpan Value { get; private set; }

        private Duration() { }

        public Duration(TimeSpan value)
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentException("Duration cannot be negative", nameof(value));
            if (value > TimeSpan.FromDays(365))
                throw new ArgumentException("Duration cannot exceed 365 days", nameof(value));
            Value = value;
        }

        public static Duration FromMinutes(int minutes) => new(TimeSpan.FromMinutes(minutes));
        public static Duration FromHours(int hours) => new(TimeSpan.FromHours(hours));
        public static Duration FromDays(int days) => new(TimeSpan.FromDays(days));

        public int TotalMinutes => (int)Value.TotalMinutes;
        public int TotalHours => (int)Value.TotalHours;
        public int TotalDays => (int)Value.TotalDays;

        public string ToReadableString()
        {
            if (Value.TotalMinutes < 60)
                return $"{Value.TotalMinutes:F0} minutes";

            if (Value.TotalHours < 24)
                return $"{Value.TotalHours:F1} hours";

            return $"{Value.TotalDays:F1} days";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator TimeSpan(Duration duration) => duration?.Value ?? TimeSpan.Zero;
        public override string ToString() => ToReadableString();
    }
}
