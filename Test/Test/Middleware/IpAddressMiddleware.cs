namespace EmployeeApiConsumer.CustomeMiddlewares
{
    public class IpAddressMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        public IpAddressMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
          string ipAddress = context.Request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress!.MapToIPv4().ToString();
            }
            context.Items["IpAddress"] = ipAddress;
            await _requestDelegate(context);
        }
    }
}
