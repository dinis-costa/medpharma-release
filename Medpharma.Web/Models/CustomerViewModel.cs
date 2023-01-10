using Medpharma.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Models
{
    public class CustomerViewModel : Customer
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
