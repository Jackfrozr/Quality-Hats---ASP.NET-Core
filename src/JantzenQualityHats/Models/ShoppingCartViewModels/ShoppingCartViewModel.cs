using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JantzenQualityHats.Models.ShoppingCartViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public decimal Gst { get; set; }
        public decimal GrandTotal { get; set; }

    }
}
