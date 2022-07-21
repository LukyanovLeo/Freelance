using System;

namespace Freelance.WebApi.Contracts.Common
{
    public class FileUploadResult
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
    }
}
