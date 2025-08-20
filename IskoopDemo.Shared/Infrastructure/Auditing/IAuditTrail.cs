using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Infrastructure.Auditing
{
    public interface IAuditTrail
    {
        Guid Id { get; }
        string TableName { get; }
        string Operation { get; } //insert, update, delete
        string PrimaryKey { get; }
        string OldValue { get; }
        string NewValue { get; }
        string ChangedColumns { get; }
        DateTime Timestamp { get; }
        string UserId { get; }
        string UserName { get; }
    }
}
