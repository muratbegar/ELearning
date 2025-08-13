using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELeraningIskoop.ServiceDefaults.Logging
{
    // Performance ölçümü için timer
    public class PerformanceTimer : IDisposable
    {
        private readonly string _operationName;
        private readonly System.Diagnostics.Stopwatch _stopwatch;
        private readonly IDisposable _logContext;

        public PerformanceTimer(string operationName)
        {
            _operationName = operationName;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logContext = LogContext.PushProperty("OperationName", operationName);
        }
        public void Dispose()
        {
            _stopwatch.Stop();

            Log.Information(
                "Operation {OperationName} completed in {Duration}ms",
                _operationName, _stopwatch.ElapsedMilliseconds);

            _logContext?.Dispose();
        }

    }
}
