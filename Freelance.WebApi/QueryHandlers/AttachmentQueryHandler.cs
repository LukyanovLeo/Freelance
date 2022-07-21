using Freelance.WebApi.Contracts.Common;
using Freelance.QueryHandlers.Interfaces;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers
{
    public class AttachmentQueryHandler: IAttachmentQueryHandler
    {
        public Task<FileContent> DownloadAttachment()
        {
            throw new System.NotImplementedException();
        }

        public Task<FileUploadResult> UploadAttachment()
        {
            throw new System.NotImplementedException();
        }
    }
}
