using System;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Medicine : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Prescription Required")]
        public bool NeedsPrescription { get; set; }

        [Required]
        public int Stock { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty ?
            $"/images/noimage.png" :
            $"/images/medicine/{ImageId}.jpg"; 
    }
}
