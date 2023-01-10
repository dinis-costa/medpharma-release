using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class MedicineViewModel : Medicine
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
