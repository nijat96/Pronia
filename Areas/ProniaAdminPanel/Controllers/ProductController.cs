using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pronia.Areas.ProniaAdminPanel.ViewModels.Product;
using Pronia.DAL;
using Pronia.Entities;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class ProductController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            var createProductVM = new CreateProductVM
            {
                Categories = categories
            };
            return View(createProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM createProductVM)
        {

            for(int i = 0; i < createProductVM.AdditionalImages.Count; i++)
            {
                if(createProductVM.AdditionalImages[i].Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError($"AdditionalImages[{i}]", "File size must be less than 2MB.");
                    createProductVM.AdditionalImages.RemoveAt(i);
                    i--;
                }
            }


            return View("ss");
        }


    }
}
