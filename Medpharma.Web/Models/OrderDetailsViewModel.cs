using Medpharma.Web.Data.Entities;
using System;
using System.Collections.Generic;

namespace Medpharma.Web.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public Customer Customer { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public int OrderSent { get; set; }
        public List<OrderDetail> OrderDetailList { get; set; }
    }
}
