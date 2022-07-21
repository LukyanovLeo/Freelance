using Freelance.WebApi.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Freelance.WebApi.Common.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(v => v.ErrorMessage)
                        .ToList();

                var responseObj = errors.Select(x => new FailureData(x, ErrorLevel.Error));

                context.Result = new BadRequestObjectResult(responseObj);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var a = "das";
        }
    }
}
