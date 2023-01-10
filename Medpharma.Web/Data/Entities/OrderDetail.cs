using System.ComponentModel.DataAnnotations;

namespace Medpharma.Web.Data.Entities
{
    public class OrderDetail : IEntity
    {
        public int Id { get; set; }

        public Medicine Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value => Price * (decimal)Quantity;
    }
}
