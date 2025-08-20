using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class ModifiedDate : ValueObject
    {

        public DateTime Value { get; private set; }

        private ModifiedDate() {}

        public ModifiedDate(DateTime value)
        {
            if(value == default)
            {
                throw new ArgumentException("Modified date cannot be default value.", nameof(value));
            }
            if (value > DateTime.UtcNow.AddMinutes(1))
            {
                throw new ArgumentException("Modified date cannot be in the future", nameof(value));
            }
            Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }


        public static ModifiedDate Now() => new(DateTime.UtcNow);

        public bool IsModifiedAfter(CreatedDate createdDate)
        {
            return Value > createdDate.Value;
        }

        public bool IsRecentlyModified(TimeSpan timespan)
        {
            return DateTime.UtcNow - Value <= timespan;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator DateTime(ModifiedDate modifiedDate) => modifiedDate?.Value ?? default;
        public static implicit operator ModifiedDate(DateTime dateTime) => new(dateTime);

        public override string ToString() => Value.ToString("yyyy-MM-ddTHH:mm:ssZ");


       
    }
}
