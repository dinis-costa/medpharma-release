using System;

namespace Medpharma.Web.Data.Entities
{
    public class Clerk : User
    {
        public string ImageFullPath => ImageId == Guid.Empty ?
        //$"https://dashpet.blob.core.windows.net/images/noimage.png" :
        //$"https://dashpet.blob.core.windows.net/owners/{ImageId}";
            $"~/images/noimage.png" :
            $"~/images/clerk/{ImageId}.jpg";

        public bool WareHouse { get; set; }
    }
}
