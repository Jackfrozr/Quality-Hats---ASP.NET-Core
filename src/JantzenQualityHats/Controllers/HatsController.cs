using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JantzenQualityHats.Data;
using JantzenQualityHats.Models;
using System.Net.Http.Headers; //Week 6
using Microsoft.AspNetCore.Hosting; //Week 6
using Microsoft.AspNetCore.Http; //Week 6
using System.IO; //Week 6
using Microsoft.AspNetCore.Authorization;

namespace JantzenQualityHats.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public HatsController(ApplicationDbContext context, IHostingEnvironment hEnv)
        {
            _context = context;
            _hostingEnv = hEnv;
        }

        // GET: Hats
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
            ViewData["IDSortParm"] = sortOrder == "ID" ? "id_desc" : "ID";

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
                case "ID":
                    hats = hats.OrderBy(s => s.HatID);
                    break;
                case "id_desc":
                    hats = hats.OrderByDescending(s => s.HatID);
                    break;
                default:
                    hats = hats.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Hat>.CreateAsync(hats.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Hats/Details/5
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

        // GET: Hats/Create
        public IActionResult Create()
        {
            //ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID");
            PopulateSupplierList();
            PopulateCategoryList();
            return View();
        }

        // POST: Hats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,Description,Image,Name,Price,SupplierID")] Hat hat, IList<IFormFile> _files)
        {
        //IMAGE
            var relativeName = "";  
            var fileName = ""; 

            if (_files.Count < 1) 
            {
                relativeName = "/Images/Default.jpg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/Images/HatImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            hat.Image = relativeName;
            PopulateCategoryList(hat.CategoryID);
            PopulateSupplierList(hat.SupplierID);
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(hat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists " + "see your system administrator.");
            }

            //ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierID", hat.SupplierID);
            return View(hat);
        }

        // GET: Hats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hat = await _context.Hats
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.HatID == id);
            if (hat == null)
            {
                return NotFound();
            }
            PopulateSupplierList(hat.SupplierID);
            PopulateCategoryList(hat.CategoryID);
            return View(hat);
        }


        // POST: Hats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, IList<IFormFile> _files)
        {
            if (id == null)
            {
                return NotFound();
            }

            //IMAGE
            var relativeName = "";
            var fileName = "";

            if (_files.Count < 1)
            {
                relativeName = "/Images/Default.jpg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/Images/HatImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            //hat.Image = relativeName;

            //Other
            var hatToUpdate = await _context.Hats
                .SingleOrDefaultAsync(c => c.HatID == id);
            hatToUpdate.Image = relativeName;
            if (await TryUpdateModelAsync<Hat>(hatToUpdate,
            "",
            c => c.CategoryID, c => c.Description,c=>c.Image, c=> c.Name, c=> c.Price, c=>c.SupplierID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                }
            }
            PopulateSupplierList(hatToUpdate.SupplierID);
            PopulateCategoryList(hatToUpdate.CategoryID);
            return View(hatToUpdate);
        }
        private void PopulateImage(object selectedSupplier = null)
        {

        }

        private void PopulateSupplierList(object selectedSupplier = null)
        {
            var supplierQuery = from d in _context.Suppliers
                                orderby d.Name
                                select d;
            ViewBag.SupplierID = new SelectList(supplierQuery.AsNoTracking(), "SupplierID", "Name",
            selectedSupplier);
        }

        private void PopulateCategoryList(object selectedCategory = null)
        {
            var categoryQuery = from d in _context.Categories
                                orderby d.Name
                                select d;
            ViewBag.CategoryID = new SelectList(categoryQuery.AsNoTracking(), "CategoryID", "Name",
            selectedCategory);
        }

        // GET: Hats/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hat = await _context.Hats
                .SingleOrDefaultAsync(m => m.HatID == id);
            if (hat == null)
            {
                return NotFound();
            }

            if(saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                "Delete failed. Try again, and if the problem persists " +
                "see your system administrator.";
            }
            return View(hat);
        }

        // POST: Hats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hat = await _context.Hats
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.HatID == id);

            if (hat == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                _context.Hats.Remove(hat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch(DbUpdateException)
            {
                TempData["HatUsed"] = "The hat being deleted has been used in previous orders.Delete those orders before trying again.";
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
        }

        private bool HatExists(int id)
        {
            return _context.Hats.Any(e => e.HatID == id);
        }
    }
}
