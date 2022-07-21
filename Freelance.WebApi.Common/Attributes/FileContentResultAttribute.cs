using Freelance.WebApi.Contracts.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Freelance.WebApi.Common.Attributes
{
    public class FileContentResultAttribute : Attribute, IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var result = (FileContent)(context.Result as ObjectResult).Value;

            context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            context.HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename*=UTF-8''" + Uri.EscapeUriString(result.FileName));

            context.Result = new FileContentResult(result.Content, result.MimeType);

            await next();
        }
    }
}
