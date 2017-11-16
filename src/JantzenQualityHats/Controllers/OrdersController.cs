using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JantzenQualityHats.Data;
using JantzenQualityHats.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JantzenQualityHats.Controllers
{
    [Authorize(Roles = "Admin,Member")]

    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.Include(i => i.User).AsNoTracking().ToListAsync());
        }

        // GET: Orders/Create
        [Authorize(Roles = "Member")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(Order order)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {

                ShoppingCart cart = ShoppingCart.GetCart(this.HttpContext);
                List<CartItem> items = cart.GetCartItems(_context);
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (CartItem item in items)
                {

                    OrderDetail detail = CreateOrderDetailForThisItem(item);
                    detail.Order = order;
                    details.Add(detail);
                    _context.Add(detail);

                }

                order.User = user;
                decimal cartTotal = ShoppingCart.GetCart(this.HttpContext).GetTotal(_context);
                order.Subtotal = cartTotal;
                decimal cartGst = (cartTotal / 100) * 15; //GST is 15%
                order.GST = cartGst;
                order.GrandTotal = cartTotal + cartGst;
                order.OrderStatus = "waiting";
                order.CustomerID = user.Id;
                order.OrderDetails = details;
                _context.SaveChanges();


                return RedirectToAction("Purchased", new RouteValueDictionary(
                new { action = "Purchased", id = order.OrderID }));
            }

            return View(order);
        }
        private OrderDetail CreateOrderDetailForThisItem(CartItem item)
        {

            OrderDetail detail = new OrderDetail();


            detail.Quantity = item.Count;
            detail.Hat = item.Hat;
            detail.UnitPrice = item.Hat.Price;

            return detail;

        }
        public async Task<IActionResult> Purchased(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(i => i.User).AsNoTracking().SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            var details = _context.OrderDetails.Where(detail => detail.Order.OrderID == order.OrderID).Include(detail => detail.Hat).ToList();

            order.OrderDetails = details;
            ShoppingCart.GetCart(this.HttpContext).EmptyCart(_context);
            return View(order);
        }




        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(i => i.User).AsNoTracking().SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            var details = _context.OrderDetails.Where(detail => detail.Order.OrderID == order.OrderID).Include(detail => detail.Hat).ToList();

            order.OrderDetails = details;

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderID == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Manually Added
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        public async Task<IActionResult> EnableDisable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            else if(order.OrderStatus == "waiting")
            {
                order.OrderStatus = "shipped";
            }
            else if (order.OrderStatus == "shipped")
            {
                order.OrderStatus = "waiting";
            }
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
