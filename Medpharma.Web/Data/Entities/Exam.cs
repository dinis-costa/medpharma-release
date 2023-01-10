using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Exam : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }
    }
}
