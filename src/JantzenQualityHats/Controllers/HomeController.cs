using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JantzenQualityHats.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "We sell high quality hats with low-low price!";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
