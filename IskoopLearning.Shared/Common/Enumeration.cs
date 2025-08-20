using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.Common
{
    public abstract class Enumeration : IComparable<Enumeration>, IEquatable<Enumeration>
    {

        public int ObjcetId { get; }
        public string Name { get; }

        protected Enumeration(int objectId, string name)
        {
            ObjcetId = objectId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString() => $"{Name} ({ObjcetId})";
        public override bool Equals(object obj) => Equals(obj as Enumeration);
        public override int GetHashCode() => HashCode.Combine(GetType(), ObjcetId);
        public bool Equals(Enumeration other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return GetType() == other.GetType() && ObjcetId == other.ObjcetId;
        }

        public int CompareTo(Enumeration? other)
        {
            if (other is null) return 1;
            return ObjcetId.CompareTo(other.ObjcetId);
        }

        public static bool operator ==(Enumeration left, Enumeration right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Enumeration left, Enumeration right) => !(left == right);

        public static bool operator <(Enumeration left, Enumeration right)
        {
            if (left is null) return right is not null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Enumeration left, Enumeration right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Enumeration left, Enumeration right) => !(left <= right);

        public static bool operator >=(Enumeration left, Enumeration right) => !(left < right);

        //reflection-based method to get all derived types

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.ObjcetId == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty", nameof(displayName));

            var matchingItem = Parse<T, string>(displayName, "display name", item =>
                string.Equals(item.Name, displayName, StringComparison.OrdinalIgnoreCase));

            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem is null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T).Name}");

            return matchingItem;
        }

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            if (firstValue is null) throw new ArgumentNullException(nameof(firstValue));
            if (secondValue is null) throw new ArgumentNullException(nameof(secondValue));

            return Math.Abs(firstValue.ObjcetId - secondValue.ObjcetId);
        }

        public static bool ContainsValue<T>(int value) where T : Enumeration
        {
            return GetAll<T>().Any(item => item.ObjcetId == value);
        }

        public static bool ContainsName<T>(string name) where T : Enumeration
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            return GetAll<T>().Any(item =>
                string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
        }


    }
}
