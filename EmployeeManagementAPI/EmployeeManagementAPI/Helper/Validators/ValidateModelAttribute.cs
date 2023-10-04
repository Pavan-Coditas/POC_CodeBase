using EmployeeManagement.Entities.Models.PayloadModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Api.Helper.Validators
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument != null)
                {
                    var validator = new EmployeeValidator();
                    var result = await validator.ValidateAsync((Employeepayload)argument);
                    if (!result.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(result.Errors);
                        return;
                    }
                }
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}