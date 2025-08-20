using IskoopDemo.Shared.Middleware.ErrorHandling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IskoopDemo.Shared.Middleware.CircuitBreakerMiddleware
{
    // Circuit Breaker Middleware for handling cascading failures
    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CircuitBreakerMiddleware> _logger;
        private readonly CircuitBreakerOptions _options;
        private CircuitState _state = CircuitState.Closed;
        private int _failureCount = 0;
        private DateTime _lastFailureTime;
        private DateTime _openedAt;

        public CircuitBreakerMiddleware(
            RequestDelegate next,
            ILogger<CircuitBreakerMiddleware> logger,
            CircuitBreakerOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.UtcNow - _openedAt > _options.OpenDuration)
                {
                    _state = CircuitState.HalfOpen;
                    _logger.LogInformation("Circuit breaker transitioned to HalfOpen state");
                }
                else
                {
                    await HandleCircuitOpenAsync(context);
                    return;
                }
            }

            try
            {
                await _next(context);

                if (_state == CircuitState.HalfOpen)
                {
                    _state = CircuitState.Closed;
                    _failureCount = 0;
                    _logger.LogInformation("Circuit breaker closed after successful request");
                }
            }
            catch (Exception ex)
            {
                await HandleFailureAsync(context, ex);
                throw;
            }
        }

        private async Task HandleFailureAsync(HttpContext context, Exception exception)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= _options.FailureThreshold)
            {
                _state = CircuitState.Open;
                _openedAt = DateTime.UtcNow;
                _logger.LogWarning("Circuit breaker opened after {FailureCount} failures", _failureCount);
            }

            if (_state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Open;
                _openedAt = DateTime.UtcNow;
                _logger.LogWarning("Circuit breaker reopened after failure in HalfOpen state");
            }
        }

        // ... existing code ...

        private async Task HandleCircuitOpenAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            context.Response.Headers.Add("Retry-After", _options.OpenDuration.TotalSeconds.ToString());

            var response = new ErrorResponse
            {
                Type = "https://errors.example.com/circuit-breaker-open",
                Title = "Service Temporarily Unavailable",
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Detail = "The service is temporarily unavailable due to high error rate",
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier,
                Extensions = new Dictionary<string, object>
                {
                    ["retryAfter"] = _options.OpenDuration.TotalSeconds
                }
            };

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private enum CircuitState
        {
            Closed,
            Open,
            HalfOpen
        }
    }
}   
