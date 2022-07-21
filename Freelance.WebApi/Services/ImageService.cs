using Freelance.Services.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Freelance.Services
{
    public class ImageService : IImageService
    {
        private const int ThumbnailWidth = 120;
        private const int WidthHeight = 120;

        public Task<byte[]> GetThumbnailImage(byte[] content)
        {
            using (var memoryStream = new MemoryStream(content))
            {
                Image image = Image.FromStream(memoryStream, true);
                var thumb = image.GetThumbnailImage(ThumbnailWidth, WidthHeight, () => false, IntPtr.Zero);

                memoryStream.SetLength(0);
                thumb.Save(memoryStream, thumb.RawFormat);

                return Task.FromResult(memoryStream.ToArray());
            }
        }
    }
}
