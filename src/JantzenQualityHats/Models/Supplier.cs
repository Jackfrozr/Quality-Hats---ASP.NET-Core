using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace JantzenQualityHats.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public string HomePhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
