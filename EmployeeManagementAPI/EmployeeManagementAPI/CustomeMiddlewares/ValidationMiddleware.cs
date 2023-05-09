using System.Text;
using EmployeeManagement.Entities.Models.PayloadModel;
using EmployeeManagement.Api.Helper.Validators;

namespace EmployeeManagement.Api.CustomeMiddlewares
{
    public class ValidateMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalRequestBody = context.Request.Body;

            try
            {
                string requestBody = "";

                // Read the request body if it's not already read
                if (context.Request.ContentLength != null && context.Request.ContentLength > 0 && context.Request.Body.CanRead)
                {
                    context.Request.EnableBuffering();
                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;
                    }
                }

                // Deserialize the request body
                var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<Employeepayload>(requestBody);

                // Validate the payload
                var validator = new EmployeeValidator();
                var result = await validator.ValidateAsync(payload!);
                if (!result.IsValid)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(result.Errors));
                    return;
                }

                // Replace the request body with the original stream
                context.Request.Body = originalRequestBody;

                await _next(context);
            }
            finally
            {
                // Replace the request body with the original stream in case of any exception
                context.Request.Body = originalRequestBody;
            }
        }
    }
}
