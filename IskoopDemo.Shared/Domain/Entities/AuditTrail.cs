using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Application.Interfaces;
using IskoopDemo.Shared.Infrastructure.Auditing;
using IskoopDemo.Shared.Infrastructure.Common;

namespace IskoopDemo.Shared.Domain.Entities
{
    public class AuditTrail : BaseEntity,IAuditTrail
    {
        public Guid Id { get; }
        public string TableName { get; }
        public string Operation { get; }
        public string PrimaryKey { get; }
        public string OldValue { get; }
        public string NewValue { get; }
        public string ChangedColumns { get; }
        public DateTime Timestamp { get; }
        public string UserId { get; }
        public string UserName { get; }

        private AuditTrail() { } // EF Constructor

        public AuditTrail(
            string tableName,
            string operation,
            string primaryKey,
            string oldValues = null,
            string newValues = null,
            string changedColumns = null,
            string userId = null,
            string userName = null)
        {
            TableName = tableName;
            Operation = operation;
            PrimaryKey = primaryKey;
            OldValue = oldValues;
            NewValue = newValues;
            ChangedColumns = changedColumns;
            Timestamp = DateTime.UtcNow;
            UserId = userId;
            UserName = userName;
        }
    }
}
