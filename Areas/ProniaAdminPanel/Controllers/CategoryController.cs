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
        public IActionResult Categories()
        {
            var categories = _context.Categories.ToList();
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

            bool existCategory = await _context.Categories.AnyAsync(c=>c.Name.Trim().ToLower()==categoryVM.Name.Trim().ToLower());

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
            _context.SaveChanges();
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [HttpPut]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            category.IsDeleted = true;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }
    }
}
