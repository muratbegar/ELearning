using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELearningIskoop.BuildingBlocks.Application.CQRS;
using MediatR;
using Serilog;

namespace ELearningIskoop.BuildingBlocks.Application.Behaviors
{
    // MediatR pipeline'ında Serilog ile structured loglama yapmak için behavior
    // Request/Response ve performans loglaması yapar
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger _logger = Log.ForContext<LoggingBehavior<TRequest, TResponse>>();

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid();

            // Sensitive data'yı loglamamak için request'i serialize etmeyelim
            // Sadece önemli bilgileri logla
            var requestInfo = ExtractRequestInfo(request);

            // Request başlangıç logu - Structured logging
            _logger.Information("Request başlatıldı. {RequestName} ile {RequestId} {@RequestId}", requestName,
                requestId, requestInfo);


            // Performance ölçümü başlat
            var stopWatch = Stopwatch.StartNew();
            try
            {
                // Request'i işle
                var response = await next();

                stopWatch.Stop();
                // Başarılı response logu
                _logger.Information(
                    "Request başarıyla tamamlandı {RequestName} RequestId: {RequestId} Süre: {Duration}ms",
                    requestName, requestId, stopWatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopWatch.Stop();

                // Hata logu - Exception ile birlikte context bilgileri
                _logger.Error(ex,
                    "Request başarısız oldu {RequestName} RequestId: {RequestId} Süre: {Duration}ms",
                    requestName, requestId, stopWatch.ElapsedMilliseconds);

                throw;
            }
        }


        // Request'ten loglama için güvenli bilgileri çıkarır
        // Sensitive data'yı (password, email vs) loglamaz
        public static object ExtractRequestInfo(TRequest request)
        {
            // BaseCommand veya BaseQuery'den tracking bilgilerini al
            if (request is BaseCommand baseCommand)
            {
                return new
                {
                    TrackingId = baseCommand.TrackingId,
                    RequestedBy = baseCommand.RequestedBy,
                    RequestedAt = baseCommand.CreatedAt
                };
            }

            if(request is BaseQuery<TRequest> baseQuery)
            {
                return new
                {
                    TrackingId = baseQuery.TrackingId,
                    RequestedBy = baseQuery.RequestedBy,
                    RequestedAt = baseQuery.CreatedAt
                };
            }

            //iğer request türleri için varsayılan bilgileri döndür
            return new
            {
                RequestType = request.GetType().Name,
                RequestData = request.ToString() // ToString() ile basit bir gösterim al
            };
        }
    }
}
