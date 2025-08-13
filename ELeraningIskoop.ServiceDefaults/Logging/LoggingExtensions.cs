using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Context;

namespace ELeraningIskoop.ServiceDefaults.Logging
{
    // Serilog logging helper'ları
    public static class LoggingExtensions
    {

        // User context'i ile loglama

        public static IDisposable EnrichWithUser(string? userId, string? userRole = null)
        {
            var enrichers = new List<IDisposable>();

            if (!string.IsNullOrEmpty(userId))
            {
                enrichers.Add(LogContext.PushProperty("UserId", userId));
            }

            if (!string.IsNullOrEmpty(userRole))
            {
                enrichers.Add(LogContext.PushProperty("UserRole", userRole));
            }

            return new CompositeDisposable(enrichers);
        }

        // Correlation ID ile loglama
        public static IDisposable EnrichWithCorrelationId(string correlationId)
        {
            return LogContext.PushProperty("CorrelationId", correlationId);
        }

        // Business operation context'i ile loglama
        public static IDisposable EnrichWithOperation(string operationName, string? operationId = null)
        {
            var enrichers = new List<IDisposable>
            {
                LogContext.PushProperty("OperationName", operationName)
            };

            if (!string.IsNullOrEmpty(operationId))
            {
                enrichers.Add(LogContext.PushProperty("OperationId", operationId));
            }

            return new CompositeDisposable(enrichers);
        }

        // Performance ölçümü ile loglama

        public static PerformanceTimer StartPerformanceTimer(string operationName)
        {
            return new PerformanceTimer(operationName);
        }

    }
}
