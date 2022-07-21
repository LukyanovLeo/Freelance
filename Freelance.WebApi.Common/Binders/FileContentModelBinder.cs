using Freelance.WebApi.Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;
using System.Threading.Tasks;

namespace Freelance.WebApi.Common.Binders
{
    public class FileContentModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var file = bindingContext.HttpContext.Request.Form.Files[bindingContext.ModelName];
            var mimeType = "";

            if (file == null)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            FileContent fileContent = new FileContent
            {
                FileName = file.FileName,
                MimeType = mimeType,
                Content = GetBytes(file)
            };

            bindingContext.Result = ModelBindingResult.Success(fileContent);
            return Task.CompletedTask;
        }

        private static byte[] GetBytes(IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
