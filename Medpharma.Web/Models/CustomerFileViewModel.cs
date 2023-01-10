using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class CustomerFileViewModel : CustomerFile
    {
        [Display(Name = "File")]
        [Required]
        public IFormFile File { get; set; }
    }
}
