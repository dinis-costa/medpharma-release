using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public interface IImageHelper
    {
        Task/*<string>*/<Guid> UploadImageAsync(IFormFile imageFile, string folder);
    }
}