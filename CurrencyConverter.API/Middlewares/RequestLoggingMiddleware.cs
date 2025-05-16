using System.Diagnostics;

namespace CurrencyConverter.API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var httpMethod = context.Request.Method;
            var endpoint = $"{context.Request.Path}{context.Request.QueryString}";
            var responseCode = context.Response.StatusCode;
            
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var responseTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("HTTP Request {@Method} {@Endpoint} responded {@StatusCode} in {@ElapsedMs}ms | ClientIP: {@ClientIP}",
                httpMethod, endpoint, responseCode, responseTimeMs, clientIp);
        }
    }
}
