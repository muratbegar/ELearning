using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace ELearningIskoop.BuildingBlocks.Application.Behaviors
{
    // MediatR pipeline'ında Serilog ile performans izleme için behavior
    // Uzun süren request'leri loglar
    //TRequest Performans izlenecek request tipi TResponse Response tipi
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger _logger = Log.ForContext<PerformanceBehavior<TRequest, TResponse>>();
        private readonly Stopwatch _timer;

        // Bu süreyi aşan request'ler warning olarak loglanır (milisaniye)
        private const int WarningThresholdMs = 500;

        // Bu süreyi aşan request'ler error olarak loglanır (milisaniye)
        private const int ErrorThresholdMs = 2000;

        public PerformanceBehavior()
        {
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {


            _timer.Start();

            var response = await next();
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            // Performance threshold'lara göre loglama
            if (elapsedMilliseconds > ErrorThresholdMs)
            {
                _logger.Error(
                    "Kritik yavaş request tespit edildi {RequestName} Süre: {Duration}ms Threshold: {Threshold}ms",
                    requestName, elapsedMilliseconds, ErrorThresholdMs);
            }
            else if (elapsedMilliseconds > WarningThresholdMs)
            {
                _logger.Warning(
                    "Yavaş request tespit edildi {RequestName} Süre: {Duration}ms Threshold: {Threshold}ms",
                    requestName, elapsedMilliseconds, WarningThresholdMs);
            }
            else
            {
                // Normal performans - debug level
                _logger.Debug(
                    "Request performance {RequestName} Süre: {Duration}ms",
                    requestName, elapsedMilliseconds);
            }

            return response;
        }

    }
}

