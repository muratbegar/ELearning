using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Common
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();
        #region Equality
        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return Equals(obj as ValueObject);
        }

        public bool Equals(ValueObject other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return GetEqualityComponents()
                .SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate(17, (current, hashCode) =>
                {
                    unchecked
                    {
                        return current * 23 + hashCode;
                    }
                });
        }
        #endregion

        #region Operators

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

        #endregion

        #region Utility Methods

        // Deep copy of the value object (override if needed)
        public virtual ValueObject Copy()
        {
            return (ValueObject)MemberwiseClone();
        }


        // String representation for debugging
        public override string ToString()
        {
            var components = GetEqualityComponents()
                .Select(x => x?.ToString() ?? "null")
                .ToArray();

            return $"{GetType().Name} [{string.Join(", ", components)}]";
        }
        #endregion
    }
}
