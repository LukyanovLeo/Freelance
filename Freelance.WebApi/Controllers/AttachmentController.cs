using Freelance.WebApi.Common.Attributes;
using Freelance.WebApi.Common.Binders;
using Freelance.WebApi.Contracts.Common;
using Freelance.QueryHandlers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Freelance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentQueryHandler _queryHandler;

        public AttachmentController(IAttachmentQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [AllowAnonymous]
        [HttpPost("uploadAttachment")]
        public Task<FileUploadResult> UploadAttachment([ModelBinder(BinderType = typeof(FileContentModelBinder))] FileContent fileContent) => _queryHandler.UploadAttachment();

        [HttpGet("downloadAttachment/{attachmentId}")]
        [FileContentResultAttribute]
        public Task<FileContent> DownloadAttachment(Guid attachmentId) => _queryHandler.DownloadAttachment();
    }
}
