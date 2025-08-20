using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.Common;

namespace IskoopDemo.Shared.Domain.Entities
{
    internal class AuditInfo : ValueObject
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }

        private AuditInfo(){}


        public AuditInfo(int userId,string userName, string ipAddress,string userAgent)
        {
            UserId = userId;
            UserName = userName;
            Timestamp = DateTime.UtcNow;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
            yield return UserName;
            yield return Timestamp;
            yield return IpAddress;
            yield return UserAgent;
        }
    }
}
