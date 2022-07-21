using Microsoft.AspNetCore.StaticFiles;

namespace Freelance.WebApi.Contracts.Common
{
    public class FileContent
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }

        public FileContent() { }

        public FileContent(byte[] content, string fileName)
        {
            Content = content;
            FileName = fileName;
            MimeType = getContentType(fileName);
        }

        private string getContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
