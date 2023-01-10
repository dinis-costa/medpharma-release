using System;

namespace Medpharma.Web.Data.Entities
{
    public class Doctor : User
    {
        public string ImageFullPath => ImageId == Guid.Empty ?
        //$"https://dashpet.blob.core.windows.net/images/noimage.png" :
        //$"https://dashpet.blob.core.windows.net/owners/{ImageId}";
            $"~/images/noimage.png" :
            $"~/images/doctor/{ImageId}.jpg";

        //FK
        public virtual Speciality Speciality { get; set; }

        public int SpecialityId { get; set; }

        public bool IsActive { get; set; }

        public bool EmergencyService { get; set; }
    }
}
