using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Middleware.Logging
{
    // Request/Response Logging Middleware
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly RequestResponseLoggingOptions _options;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger,
            RequestResponseLoggingOptions options = null)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _options = options ?? new RequestResponseLoggingOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipLogging(context))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var requestLog = await LogRequestAsync(context);
            var originalBodyStream = context.Response.Body;

            using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                await LogResponseAsync(context, responseBody, originalBodyStream, requestLog, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task<RequestLog> LogRequestAsync(HttpContext context)
        {
            var request = context.Request;
            var requestLog = new RequestLog
            {
                TraceId = context.TraceIdentifier,
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString.ToString(),
                Scheme = request.Scheme,
                Host = request.Host.ToString(),
                Headers = GetHeaders(request.Headers),
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request.Headers["User-Agent"].ToString(),
                Timestamp = DateTime.UtcNow
            };

            // Add user information if authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                requestLog.UserId = context.User.FindFirst("sub")?.Value;
                requestLog.Username = context.User.Identity.Name;
            }

          

            // Create structured log
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = requestLog.TraceId,
                ["RequestMethod"] = requestLog.Method,
                ["RequestPath"] = requestLog.Path
            }))
            {
                _logger.LogInformation("HTTP Request Started: {Method} {Path}{QueryString}",
                    requestLog.Method,
                    requestLog.Path,
                    requestLog.QueryString);

                if (_options.LogHeaders)
                {
                    _logger.LogDebug("Request Headers: {@Headers}", requestLog.Headers);
                }
            }

            return requestLog;
        }

        private async Task LogResponseAsync(
            HttpContext context,
            MemoryStream responseBody,
            Stream originalBodyStream,
            RequestLog requestLog,
            long elapsedMs)
        {
            var response = context.Response;
            var responseLog = new ResponseLog
            {
                TraceId = context.TraceIdentifier,
                StatusCode = response.StatusCode,
                Headers = GetHeaders(response.Headers),
                ElapsedMilliseconds = elapsedMs,
                Timestamp = DateTime.UtcNow
            };

            // Log response body if configured
            if (_options.LogResponseBody && IsTextContentType(response.ContentType))
            {
                responseBody.Position = 0;
                responseLog.Body = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Position = 0;
            }

            // Copy response body back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;

            // Determine log level based on status code
            var logLevel = DetermineLogLevel(response.StatusCode);

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = responseLog.TraceId,
                ["StatusCode"] = responseLog.StatusCode,
                ["ElapsedMs"] = responseLog.ElapsedMilliseconds
            }))
            {
                _logger.Log(logLevel,
                    "HTTP Request Completed: {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMs}ms",
                    requestLog.Method,
                    requestLog.Path,
                    requestLog.QueryString,
                    responseLog.StatusCode,
                    responseLog.ElapsedMilliseconds);

                if (_options.LogResponseHeaders)
                {
                    _logger.LogDebug("Response Headers: {@Headers}", responseLog.Headers);
                }

                // Log slow requests
                if (elapsedMs > _options.SlowRequestThresholdMs)
                {
                    _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMs}ms",
                        requestLog.Method, requestLog.Path, elapsedMs);
                }
            }
        }

        private bool ShouldSkipLogging(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Skip health checks and metrics endpoints
            if (_options.ExcludePaths.Any(excludePath => path?.Contains(excludePath.ToLower()) == true))
            {
                return true;
            }

            // Skip static files
            if (_options.ExcludeStaticFiles && IsStaticFile(path))
            {
                return true;
            }

            return false;
        }

        private bool IsStaticFile(string path)
        {
            var staticExtensions = new[] { ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2" };
            return staticExtensions.Any(ext => path?.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == true);
        }

        private bool IsTextContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            var textContentTypes = new[]
            {
                "application/json",
                "application/xml",
                "text/",
                "application/x-www-form-urlencoded"
            };

            return textContentTypes.Any(type => contentType.Contains(type, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<string> ReadBodyAsync(Stream body)
        {
            if (body.Length > _options.MaxBodyLogSize)
            {
                return $"[Body too large to log: {body.Length} bytes]";
            }

            body.Position = 0;
            using var reader = new StreamReader(body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var bodyContent = await reader.ReadToEndAsync();

            // Mask sensitive data
            bodyContent = MaskSensitiveData(bodyContent);

            body.Position = 0;
            return bodyContent;
        }

        private string MaskSensitiveData(string content)
        {
            if (string.IsNullOrEmpty(content) || !_options.MaskSensitiveData)
                return content;

            foreach (var pattern in _options.SensitiveDataPatterns)
            {
                var regex = new System.Text.RegularExpressions.Regex(
                    $@"(""{pattern}"":\s*"")(.*?)("")",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                content = regex.Replace(content, "$1[REDACTED]$3");
            }

            return content;
        }

        private Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
        {
            var headerDict = new Dictionary<string, string>();

            foreach (var header in headers)
            {
                // Skip sensitive headers
                if (_options.SensitiveHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
                {
                    headerDict[header.Key] = "[REDACTED]";
                }
                else
                {
                    headerDict[header.Key] = header.Value.ToString();
                }
            }

            return headerDict;
        }

        private LogLevel DetermineLogLevel(int statusCode)
        {
            return statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                _ => LogLevel.Information
            };
        }
    }
}
