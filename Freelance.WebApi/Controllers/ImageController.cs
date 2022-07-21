using Freelance.WebApi.Common.Attributes;
using Freelance.WebApi.Common.Binders;
using Freelance.WebApi.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Freelance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        //[Authorize]
        //[HttpPost("getThumbnailImage")]
        //[FileContentResult]
        //public Task<FileContent> GetThumbnailImage([ModelBinder(BinderType = typeof(FileContentModelBinder))] FileContent fileContent) => _queryHandler.Login(request.Email, ipAddress());
    }
}
