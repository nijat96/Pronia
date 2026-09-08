using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModel;

namespace Pronia.Controllers
{
    public class ProductController(AppDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            Product? product = await _context.Products.Include(p => p.Category).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            List<Product> relatedProducts = await _context.Products.Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsDeleted == false).Include(p => p.ProductImages).ToListAsync();

            ProductVM productVM = new()
            {
                Product = product,
                RelatedProducts = relatedProducts
            };
            return View(productVM);
        }
    }
}
