using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public interface IAuditLogger
    {
        Task LogAsync(AuditEntry entry);
    }
}
