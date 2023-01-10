using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class OrderDetailTemp : IEntity
    {
        public int Id { get; set; }

        public Medicine Medicine { get; set; }

        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value => Price * (decimal)Quantity;

        public Customer Customer { get; set; }

    }
}
