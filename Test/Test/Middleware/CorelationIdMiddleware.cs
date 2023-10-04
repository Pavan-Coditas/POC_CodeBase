using Microsoft.Extensions.Primitives;

namespace EmployeeApiConsumer.Middleware
{
    public class CorelationIdMiddleware
    {
        private const string CorrelationIdHeaderKey = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public CorelationIdMiddleware(RequestDelegate next,
        ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory.CreateLogger<CorelationIdMiddleware>();
        }
        public async Task Invoke(HttpContext httpContext)
        {
            string correlationId = null!;
            if (httpContext.Request.Headers.TryGetValue(
            CorrelationIdHeaderKey, out StringValues correlationIds))
            {
                correlationId = correlationIds.FirstOrDefault(k => k.Equals(CorrelationIdHeaderKey))!;
                _logger.LogInformation($"CorrelationId from Request Header:  {correlationId}    ");
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Add(CorrelationIdHeaderKey,
                correlationId);
                _logger.LogInformation($"Generated CorrelationId:{correlationId}  ");
            }
            httpContext.Response.OnStarting(() =>
            {
                if (!httpContext.Response.Headers.
                TryGetValue(CorrelationIdHeaderKey,
                out correlationIds))
                    httpContext.Response.Headers.Add(
                    CorrelationIdHeaderKey, correlationId);
                return Task.CompletedTask;
            });
            await _next.Invoke(httpContext);
        }
    }
}
