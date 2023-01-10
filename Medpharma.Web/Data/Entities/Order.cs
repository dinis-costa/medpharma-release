using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Medpharma.Web.Data.Entities
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Customer Customer { get; set; }
        // Relação de 1:M - Order details são as linhas que aparecem no layout/lista das orders adicionadas pelo user
        public IEnumerable<OrderDetail> Items { get; set; }
        public int Lines => Items == null ? 0 : Items.Count();
        public double Quantity => Items == null ? 0 : Items.Sum(i => i.Quantity); // Se for null não há encomendas


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CValue => Items == null ? 0 : Items.Sum(i => i.Value);

        public DateTime? OrderDateLocal => this.OrderDate == null ? null : this.OrderDate.ToLocalTime();

        public int OrderSent { get; set; } = 0;
    }
}
