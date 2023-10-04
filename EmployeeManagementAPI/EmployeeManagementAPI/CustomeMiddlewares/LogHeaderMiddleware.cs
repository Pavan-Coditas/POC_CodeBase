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

        //This code is an example of an asynchronous middleware in ASP.NET Core. It is used to add a custom header to the request and set a correlation ID. The correlation
        // ID is either taken from the request header or generated as a new Guid. The code also sets the UserName and IpAddress properties in the LogContext. Finally, the code
        // calls the next middleware in the pipeline.
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
            LogContext.PushProperty("UserName", UserContext.UserName);
            LogContext.PushProperty("IpAddress", UserContext.IpAddress);
            await this._next(context);
        }

    }
}
