using EmployeeManagement.Context;
using Serilog.Context;

namespace EmployeeManagement.Api.Middleware
{
    public class LogHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public LogHeaderMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers["CorrelationId"];
            string sessionId;
            if (header.Count > 0)
            {
                sessionId = header[0]!;
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
            }
            context.Items["CorrelationId"] = sessionId;
            context.Request.Headers.Add("my-custom-correlation-id", sessionId);
            LogContext.PushProperty("UserName",UserContext.UserName);
            LogContext.PushProperty("IpAddress",UserContext.IpAddress);
            await this._next(context);
        }

    }
}
