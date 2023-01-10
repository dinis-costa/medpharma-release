using Medpharma.Web.Data.Entities;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class CheckoutViewModel
    {

        public Customer Customer { get; set; }

        public List<OrderDetailTemp> OrderDetailTempList { get; set; }
    }
}
