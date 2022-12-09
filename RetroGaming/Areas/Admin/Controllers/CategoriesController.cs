using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RetroGaming.Areas.Admin.Models;
using RetroGaming.Areas.Admin.Models.Interfaces;
using RetroGaming.Areas.Identity.Data;
using RetroGaming.Models;

namespace RetroGaming.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class CategoriesController: Controller
  {
    private readonly RetroGamingDataContext _context;
    private readonly IImageService _imageService;

    public CategoriesController( RetroGamingDataContext context, IImageService imageService )
    {
      _context = context;
      _imageService = imageService;
    }

    // GET: Admin/Categories
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Index()
    {
      return View(await _context.Categories.ToListAsync());
    }

    // GET: Admin/Categories/Details/5
    [Authorize(Roles = "admin, Editor")]
    public async Task<IActionResult> Details( int? id )
    {
      if (id == null || _context.Categories == null)
      {
        return NotFound();
      }

      var category = await _context.Categories
          .FirstOrDefaultAsync(m => m.Id == id);
      if (category == null)
      {
        return NotFound();
      }

      return View(category);
    }

    // GET: Admin/Categories/Create
    [Authorize(Roles = "Admin, Editor")]
    public IActionResult Create()
    {
      return View();
    }

    // POST: Admin/Categories/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Create( [Bind("Id,Name,Description,ImagePath")] Category category )
    {
      if (ModelState.IsValid)
      {
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SavePicture( ImageFileModel imageModel )
    {
      var url = Request.GetDisplayUrl();

      ViewBag.ImageModel = imageModel;

      // if it already exists, that means we're updating it, so we wanna return to index
      // if it doesn't exist, we're creating it and we need to return to create
      bool doesItAlreadyExist = _imageService.DoesImageExist(imageModel.File);

      var azureFile = _imageService.UploadImageToAzure(imageModel.File);

      ViewBag.ImageUri = azureFile.Result.Url;

      //if (doesItAlreadyExist)
      //{
      //  return RedirectToAction(nameof(Edit));
      //}
      return View("Create");
    }

    // GET: Admin/Categories/Edit/5
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit( int? id )
    {
      if (id == null || _context.Categories == null)
      {
        return NotFound();
      }

      var category = await _context.Categories.FindAsync(id);
      if (category == null)
      {
        return NotFound();
      }
      return View(category);
    }

    // POST: Admin/Categories/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit( int id, [Bind("Id,Name,Description,ImagePath")] Category category )
    {
      if (id != category.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(category);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!CategoryExists(category.Id))
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
      return View(category);
    }

    // GET: Admin/Categories/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete( int? id )
    {
      if (id == null || _context.Categories == null)
      {
        return NotFound();
      }

      var category = await _context.Categories
          .FirstOrDefaultAsync(m => m.Id == id);
      if (category == null)
      {
        return NotFound();
      }

      return View(category);
    }

    // POST: Admin/Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed( int id )
    {
      if (_context.Categories == null)
      {
        return Problem("Entity set 'RetroGamingDataContext.Categories'  is null.");
      }
      var category = await _context.Categories.FindAsync(id);
      if (category != null)
      {
        _context.Categories.Remove(category);

        //delete all associated products if you get rid of a category
        var productQuery = await (from item in _context.Products
                                  where item.CategoryId == id
                                  select item).ToListAsync();
        foreach (var item in productQuery)
        {
          _context.Products.Remove(item);
        }

        // get blobname out of the imagepath
        string blobUrl = category.ImagePath;
        int pos = blobUrl.LastIndexOf("/") + 1;
        string blobName = blobUrl.Substring(pos, blobUrl.Length - pos);

        // delete blob from blobstorage
        _imageService.DeleteImageFromAzure(blobName);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists( int id )
    {
      return _context.Categories.Any(e => e.Id == id);
    }
  }
}
