using Freelance.WebApi.Contracts.Common;
using Freelance.QueryHandlers.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System;

namespace Freelance.QueryHandlers
{
    public class ImageQueryHandler : IImageQueryHandler
    {
        public Task<FileContent> GetThumbnailImage(FileContent fileContent)
        {
            using (var memoryStream = new MemoryStream(fileContent.Content))
            {
                Image image = Image.FromStream(memoryStream, true);
                //var thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

                memoryStream.SetLength(0);
                //thumb.Save(memoryStream, thumb.RawFormat);

                return Task.FromResult(new FileContent
                {
                    Content = memoryStream.ToArray()
                });
            }
        }
    }
}
