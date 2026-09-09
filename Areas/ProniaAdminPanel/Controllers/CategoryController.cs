using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdminPanel.ViewModels.Category;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class CategoryController(AppDbContext _context) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }

            bool existCategory = await _context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == categoryVM.Name.Trim().ToLower());

            if (existCategory)
            {
                ModelState.AddModelError("Name", "Category Already Exist");
                return View(categoryVM);
            }

            Category category = new()
            {
                Name = categoryVM.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            UpdateCategoryVM updateCategoryVM = new()
            {
                Id = category.Id,
                Name = category.Name
            };
            return View(updateCategoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM updateCategoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updateCategoryVM);
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            bool existCategory = await _context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == updateCategoryVM.Name.Trim().ToLower());
            if (existCategory)
            {
                ModelState.AddModelError("Name", "Category Already Exist");
                return View(updateCategoryVM);
            }
            category.Name = updateCategoryVM.Name;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.IsDeleted = true;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }
        public async Task<IActionResult> Restore(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.IsDeleted = false;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }
    }
}
