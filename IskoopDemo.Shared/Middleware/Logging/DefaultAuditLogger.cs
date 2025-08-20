using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class DefaultAuditLogger : IAuditLogger
    {
        private readonly ILogger<DefaultAuditLogger> _logger;

        public DefaultAuditLogger(ILogger<DefaultAuditLogger> logger)
        {
            _logger = logger;
        }

        public Task LogAsync(AuditEntry entry)
        {
            _logger.LogInformation("Audit log: {@AuditEntry}", entry);
            // Here you would persist to your audit log storage
            return Task.CompletedTask;
        }


    }
}
