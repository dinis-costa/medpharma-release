using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medpharma.Web.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task/*<string>*/<Guid> UploadImageAsync(IFormFile imageFile, string folder)
        {
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";

            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            //return $"~/images/{folder}/{file}";
            return Guid.Parse(guid);
        }
    }
}