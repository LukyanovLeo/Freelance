using Freelance.WebApi.Contracts.Common;
using System.Threading.Tasks;

namespace Freelance.QueryHandlers.Interfaces
{
    public interface IImageQueryHandler
    {
        Task<FileContent> GetThumbnailImage(FileContent fileContent);
    }
}
