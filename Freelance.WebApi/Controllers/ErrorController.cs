using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Freelance.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {


        //[Route("error")]
        //public async Task<FailureData> Error()
        //{
        //    var context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        //    var exception = context.Error;

        //    Response.StatusCode = 500;

        //    return new FailureData(exception);
        //}
    }
}