using Freelance.WebApi.Contracts.Common;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers.Interfaces
{
    public interface IAttachmentQueryHandler
    {
        Task<FileUploadResult> UploadAttachment();
        Task<FileContent> DownloadAttachment();
    }
}
