using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroGaming.Areas.Identity.Data;
using RetroGaming.Models;

namespace RetroGaming.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class ProductsController: Controller
  {
    private readonly RetroGamingDataContext _context;

    public ProductsController( RetroGamingDataContext context )
    {
      _context = context;
    }

    // GET: Admin/Products
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Index( int? categoryId )
    {
      if (categoryId != null)
      {
        ViewBag.CategoryId = categoryId;

        List<Product> list = await (from itm in _context.Products.Include(p => p.Category)
                                    where itm.CategoryId == categoryId
                                    select new Product
                                    {
                                      CategoryId = (int)categoryId,
                                      Name = itm.Name,
                                      Id = itm.Id,
                                      Condition = itm.Condition,
                                      Category = itm.Category,
                                      Description = itm.Description,
                                      ImagePath = itm.ImagePath,
                                      Price = itm.Price,
                                      Rating = itm.Rating
                                    }).ToListAsync();

        ViewBag.CategoryId = categoryId;
        return View(list);
      }

      return View(await _context.Products.Include(p => p.Category).ToListAsync());
    }

    // GET: Admin/Products/Details/5
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Details( int? id )
    {
      if (id == null || _context.Products == null)
      {
        return NotFound();
      }

      var product = await _context.Products
          .FirstOrDefaultAsync(m => m.Id == id);

      var catid = product.CategoryId;

      var category = await _context.Categories.Where(cat => cat.Id == catid).FirstOrDefaultAsync();
      product.Category = category;

      if (product == null)
      {
        return NotFound();
      }

      return View(product);
    }

    // GET: Admin/Products/Create
    [Authorize(Roles = "Admin, Editor")]
    public IActionResult Create()
    {
      var a = Request.QueryString.Value;
      var categoryid = a.Split(new char[] { '=' }, 2);
      ViewBag.CategoryID = categoryid[ 1 ];
      return View();
    }

    // POST: Admin/Products/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Create( [Bind("Id,Name,Price,Description,Condition,Rating,ImagePath,CategoryId")] Product product )
    {
      // when new product is made, i need to create a new CategoryProduct object so i have reference between category and product
      var cat = await _context.Categories.Where(cat => cat.Id == product.CategoryId).FirstOrDefaultAsync();

      if (cat == null)
        return View("Index");


      if (ModelState.IsValid)
      {
        product.Category = cat;
        // also need to add this product to the category's product list
        cat.ProductsList.Add(product);

        _context.Add(product);
        await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Index));
        return View(product);
      }

      return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Products/Edit/5
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit( int? id )
    {
      if (id == null || _context.Products == null)
      {
        return NotFound();
      }

      var product = await _context.Products.FindAsync(id);
      if (product == null)
      {
        return NotFound();
      }
      return View(product);
    }

    // POST: Admin/Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit( int id, [Bind("Id,Name,Price,Description,Condition,Rating,ImagePath")] Product product )
    {
      if (id != product.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(product);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ProductExists(product.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View(product);
    }

    // GET: Admin/Products/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete( int? id )
    {
      if (id == null || _context.Products == null)
      {
        return NotFound();
      }

      var product = await _context.Products
          .Include(p => p.Category)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (product == null)
      {
        return NotFound();
      }

      return View(product);
    }

    // POST: Admin/Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed( int id )
    {
      if (_context.Products == null)
      {
        return Problem("Entity set 'RetroGamingDataContext.Products'  is null.");
      }
      var product = await _context.Products.FindAsync(id);
      if (product != null)
      {
        _context.Products.Remove(product);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool ProductExists( int id )
    {
      return _context.Products.Any(e => e.Id == id);
    }
  }
}
