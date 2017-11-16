using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JantzenQualityHats.Data;

namespace JantzenQualityHats.Models
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
             
            //HAT
            if (context.Hats.Any())
            {
                return; // DB has been seeded
            }

            //CATEGORIES
            var categories = new Category[]
            {
                new Category{Name="Men's Hat"},
                new Category{Name="Women's Hat"},
            };
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            //SUPPLIER
            var suppliers = new Supplier[]
            {
                new Supplier{Name="NCR Fashion",HomePhoneNumber="999-999",WorkPhoneNumber="999-999",MobilePhoneNumber="999-999",EmailAddress="NCRREP@vegas.com"},
                new Supplier{Name="Boomer Style",HomePhoneNumber="999-999",WorkPhoneNumber="999-999",MobilePhoneNumber="999-999",EmailAddress="BoomerGotStyle@vegas.com"},
            };
            foreach (Supplier s in suppliers)
            {
                context.Suppliers.Add(s);
            }
            context.SaveChanges();

            var hats = new Hat[]
            {
                new Hat{Name="Beret",Price=30,Description="A beret is a soft round cap, usually of wool felt, with a flat crown that tilts to one side.", Image="", CategoryID=1,SupplierID=1},
                new Hat{Name="Boomers cap",Price=20,Description="It is an olive green Garrison cap with a medal and several ribbons pinned on the sides.", Image="", CategoryID=1,SupplierID=2},
            };
            foreach (Hat h in hats)
            {
                context.Hats.Add(h);
            }
            context.SaveChanges();
        }
    }
}
