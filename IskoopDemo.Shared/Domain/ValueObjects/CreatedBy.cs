using IskoopDemo.Shared.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Domain.ValueObjects
{
    public class CreatedBy : ValueObject
    {

        public int UserId { get; private set; }
        public string UserName { get; private set; }

        private CreatedBy() { }

        public CreatedBy(int userId, string userName)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(userName));
            }
            UserId = userId;
            UserName = userName?.Trim();
        }

        public bool IsSystem() => UserId == 1;
        public bool IsUser(int userId) => UserId > 1;



        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
            yield return UserName;
        }

        public static implicit operator int(CreatedBy createdBy) => createdBy.UserId;
        public static implicit operator CreatedBy(int userId) => new();

        public override string ToString() => string.IsNullOrWhiteSpace(UserName) ? UserId.ToString() : $"{UserName} ({UserId})";
    }
}
