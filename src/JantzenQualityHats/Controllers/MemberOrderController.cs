using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JantzenQualityHats.Data;
using JantzenQualityHats.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JantzenQualityHats.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberOrderController : Controller
    {
        private readonly ApplicationDbContext _context;


        public MemberOrderController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: MemberOrder
        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> personalorder = ReturnAllOrder().Result;
            return View(personalorder);
        }

        private async Task<IEnumerable<Order>> ReturnAllOrder()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Order> order = _context.Orders
            .Where(x => x.CustomerID.Contains(userId))
            .ToList();
            return order;
        }

        // GET: MemberOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
