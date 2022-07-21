using System.Threading.Tasks;

namespace Freelance.Services.Interfaces
{
    public interface IImageService
    {
        Task<byte[]> GetThumbnailImage(byte[] content);
    }
}
