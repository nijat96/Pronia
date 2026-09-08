using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModel;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController( AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.Where(s => s.IsDeleted == false).OrderByDescending(s=>s.CreatedAt).ToListAsync();
            List<Product> products= await _context.Products.Where(p=>p.IsDeleted==false).Include(p=>p.ProductImages).ToListAsync();

            HomeVM homeVM = new HomeVM()
            {
                Sliders = sliders,
                Products = products
            };

            return View(homeVM);
        }
    }
}
