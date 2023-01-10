using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class Speciality : IEntity
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
