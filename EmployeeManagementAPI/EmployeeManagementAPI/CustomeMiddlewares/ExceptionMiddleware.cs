namespace EmployeeManagement.Api.CustomeMiddlewares
{
    using EmployeeManagement.Entities.Models.ResponseModel;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Serilog;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionMiddleware()
        {
            _logger = Log.ForContext<ExceptionMiddleware>();
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected Error Occured {ex.Message}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Create a custom error response
                var errorResponse = new ResponseModel()
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                };

                // Serialize the error response to JSON and write it to the response body
                var json = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
