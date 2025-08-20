using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IskoopDemo.Shared.Middleware.Logging
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuditLogger _auditLogger;
        private readonly AuditLoggingOptions _options;

        public AuditLoggingMiddleware(
            RequestDelegate next,
            IAuditLogger auditLogger,
            AuditLoggingOptions options = null)
        {
            _next = next;
            _auditLogger = auditLogger;
            _options = options ?? new AuditLoggingOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!ShouldAudit(context))
            {
                await _next(context);
                return;
            }

            //var auditEntry = await CreateAuditEntryAsync(context);

            //try
            //{
            //    await _next(context);
            //    auditEntry.Success = true;
            //    auditEntry.StatusCode = context.Response.StatusCode;
            //}
            //catch (Exception ex)
            //{
            //    auditEntry.Success = false;
            //    auditEntry.ErrorMessage = ex.Message;
            //    throw;
            //}
            //finally
            //{
            //    auditEntry.CompletedAt = DateTime.UtcNow;
            //    await _auditLogger.LogAsync(auditEntry);
            //}
        }

        private bool ShouldAudit(HttpContext context)
        {
            // Only audit state-changing operations
            var method = context.Request.Method.ToUpper();
            return method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH";
        }
    }
}
