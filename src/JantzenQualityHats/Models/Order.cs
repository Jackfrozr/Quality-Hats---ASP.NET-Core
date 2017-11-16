using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JantzenQualityHats.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerID { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GST { get; set; }
        public decimal GrandTotal { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public ApplicationUser User { get; set; }

    }
}
