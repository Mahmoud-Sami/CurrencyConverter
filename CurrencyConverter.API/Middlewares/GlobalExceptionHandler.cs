using System.Net;
using CurrencyConverter.Core.Exceptions;

namespace CurrencyConverter.API.Middlewares
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BusinessException ex)
            {
                await PrepareHttpResponse(context.Response, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error");
                await PrepareHttpResponse(context.Response);
            }
        }

        private async Task PrepareHttpResponse(HttpResponse httpResponse, BusinessException exception)
        {
            httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
            httpResponse.ContentType = "application/json";
            var result = new { Success = false, exception.Message };
            await httpResponse.WriteAsJsonAsync(result);
        }

        private async Task PrepareHttpResponse(HttpResponse httpResponse, string message = "An unexpected error has occured")
        {
            httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpResponse.ContentType = "application/json";
            var result = new { Success = false, message };
            await httpResponse.WriteAsJsonAsync(result);
        }

    }

}
