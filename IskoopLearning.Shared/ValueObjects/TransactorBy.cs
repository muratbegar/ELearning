using IskoopLearning.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopLearning.Shared.ValueObjects
{
    public class TransactorBy : ValueObject
    {
        private const int SystemUserId = 1; // Assuming 1 is the system user ID
        private const string SystemUserName = "System";

        public int UserId { get; private set; }
        public string UserName { get; private set; }

        // EF Core için protected constructor
        protected TransactorBy()
        {
            // EF Core için gerekli, initial values atanabilir
            UserId = SystemUserId;
            UserName = SystemUserName;
        }

        public TransactorBy(int userId, string userName)
        {
            if(userId <=0)
                throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

            if(string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName cannot be null or empty.", nameof(userName));

            UserId = userId;
            UserName = userName;
        }

        public bool IsSystem() => UserId == SystemUserId;
        public bool IsSameUser(int userId) => UserId == userId;
        public bool IsSameUser(TransactorBy other) =>
            other != null && UserId == other.UserId;


        public TransactorBy ChangeUserName(string newUserName)
        {
            if (string.IsNullOrWhiteSpace(newUserName))
                throw new ArgumentException("User name cannot be null or empty.", nameof(newUserName));

            return new TransactorBy(UserId, newUserName);
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
           yield return UserId;
            yield return UserName;
        
        }

        public static implicit operator int(TransactorBy createdBy) =>
            createdBy?.UserId ?? throw new ArgumentNullException(nameof(createdBy));

        public static explicit operator TransactorBy((int userId, string userName) tuple) =>
            new TransactorBy(tuple.userId, tuple.userName);

        public static explicit operator TransactorBy(int userId) =>
            throw new InvalidCastException("Cannot convert int to CreatedBy without user name. Use explicit conversion with tuple.");

        public static TransactorBy System() => new TransactorBy(SystemUserId, SystemUserName);

        public static TransactorBy FromValues(int userId, string userName) =>
            new TransactorBy(userId, userName);

        public static bool TryCreate(int userId, string userName, out TransactorBy result)
        {
            result = null;

            if (userId <= 0 || string.IsNullOrWhiteSpace(userName))
                return false;

            try
            {
                result = new TransactorBy(userId, userName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() =>
            $"{UserName} ({UserId})";

        public bool Equals(int userId) => UserId == userId;

        public int CompareTo(TransactorBy other) =>
            other == null ? 1 : UserId.CompareTo(other.UserId);
    }
}
