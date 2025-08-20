using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class ModifiedBy : ValueObject
    {

        public int UserId { get; private set; }
        public string UserName { get; private set; }

        private ModifiedBy() { }

        public ModifiedBy(int userId, string userName)
        {
            if (userId==null)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            UserId = userId;
            UserName = userName?.Trim();
        }

        public bool IsSystem() => UserId == 1;
        public bool IsUser(int userId) => UserId > 1;
        public bool IsSameUser(int createdBy) => createdBy == UserId;


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
            yield return UserName;
        }

        public static implicit operator int(ModifiedBy modifiedBy) => modifiedBy.UserId;
        public static implicit operator ModifiedBy(int userId) => new();

        public override string ToString() => string.IsNullOrWhiteSpace(UserName) ? UserId.ToString() : $"{UserName} ({UserId})";
    }
}
