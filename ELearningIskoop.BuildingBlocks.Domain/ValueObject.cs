namespace ELearningIskoop.BuildingBlocks.Domain
{
    // Value Object'ler için temel sınıf
    // Value Object'ler kimliksiz, değişmez nesnelerdir
    // Eşitlik kontrolü tüm property'lere göre yapılır
    public abstract class ValueObject
    {

        // Eşitlik kontrolü için kullanılan atomic value'ları döner
        // Alt sınıflar bu metodu override etmeli

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;
            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        // Value Object kopyalama helper metodu
        protected static bool EqualOperator(ValueObject? left, ValueObject? right)
        {
            if(ReferenceEquals(left,null) ^ ReferenceEquals(right,null))
                return false;
            return ReferenceEquals(left, right) || (left?.Equals(right) ?? false);
        }

        protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
        {
            return !EqualOperator(left, right);
        }
    }
}
