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

namespace JantzenQualityHats.Controllers
{
    [AllowAnonymous]
    [Authorize(Roles = "Member")]
    public class MemberHatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberHatsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: MemberHats
        public async Task<IActionResult> Index(
            string sortOrder, 
            string searchString,
            string currentFilter,
            int? page)
        {
            //Sort
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["SupplierSortParm"] = sortOrder == "Supplier" ? "supplier_desc" : "Supplier";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            //Page
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            //Get Hats
            var qualityHatContext = _context.Hats.Include(h => h.Supplier)
                                                 .Include(e => e.Category);
            var hats = from s in qualityHatContext
                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                hats = hats.Where(s => s.Name.Contains(searchString)
                || s.Supplier.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    hats = hats.OrderByDescending(s => s.Name);
                    break;
                case "Category":
                    hats = hats.OrderBy(s => s.CategoryID);
                    break;
                case "category_desc":
                    hats = hats.OrderByDescending(s => s.CategoryID);
                    break;
                case "Supplier":
                    hats = hats.OrderBy(s => s.SupplierID);
                    break;
                case "supplier_desc":
                    hats = hats.OrderByDescending(s => s.SupplierID);
                    break;
                case "Price":
                    hats = hats.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    hats = hats.OrderByDescending(s => s.Price);
                    break;
                default:
                    hats = hats.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Hat>.CreateAsync(hats.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: MemberHats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hat = await _context.Hats.SingleOrDefaultAsync(m => m.HatID == id);
            if (hat == null)
            {
                return NotFound();
            }

            return View(hat);
        }

        // GET: MemberHats/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID");
            return View();
        }

        // POST: MemberHats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HatID,CategoryID,Description,Image,Name,Price,StartDate,SupplierID")] Hat hat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", hat.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", hat.SupplierID);
            return View(hat);
        }

        // GET: MemberHats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hat = await _context.Hats.SingleOrDefaultAsync(m => m.HatID == id);
            if (hat == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", hat.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", hat.SupplierID);
            return View(hat);
        }

        // POST: MemberHats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HatID,CategoryID,Description,Image,Name,Price,StartDate,SupplierID")] Hat hat)
        {
            if (id != hat.HatID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HatExists(hat.HatID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryID", hat.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", hat.SupplierID);
            return View(hat);
        }

        // GET: MemberHats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hat = await _context.Hats.SingleOrDefaultAsync(m => m.HatID == id);
            if (hat == null)
            {
                return NotFound();
            }

            return View(hat);
        }

        // POST: MemberHats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hat = await _context.Hats.SingleOrDefaultAsync(m => m.HatID == id);
            _context.Hats.Remove(hat);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool HatExists(int id)
        {
            return _context.Hats.Any(e => e.HatID == id);
        }
    }
}
