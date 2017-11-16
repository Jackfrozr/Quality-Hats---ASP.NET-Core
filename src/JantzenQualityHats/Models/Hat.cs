using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JantzenQualityHats.Models
{
    public class Hat
    {
        public int HatID { get; set; }
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }
        public int Price { get; set; }
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string Description { get; set; }
        public string Image { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }
        public Category Category { get; set; }
        public DateTime StartDate { get; set; }
    }
}
