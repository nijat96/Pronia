using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Categories");
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
