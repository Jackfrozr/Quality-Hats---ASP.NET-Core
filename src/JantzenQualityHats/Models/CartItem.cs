using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JantzenQualityHats.Models
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }
        public string CartID { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
        public Hat Hat { get; set; }
    }
}
