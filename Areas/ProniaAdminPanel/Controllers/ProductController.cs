using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pronia.Areas.ProniaAdminPanel.ViewModels.Product;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Enum;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class ProductController(AppDbContext _context,IWebHostEnvironment _env) : Controller
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
            createProductVM.Categories = await _context.Categories.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(createProductVM);
            }

            if (!createProductVM.MainImage.IsImage())
            {
                ModelState.AddModelError("MainImage", "Please select a valid image file.");
                return View(createProductVM);
            }
            if (!createProductVM.HoverImage.IsImage())
            {
                ModelState.AddModelError("HoverImage", "Please select a valid image file.");
                return View(createProductVM);
            }

            if (createProductVM.MainImage.ValidSize(FileSize.MB, 3))
            {
                ModelState.AddModelError("MainImage", "The image size must be less than 3 MB.");
                return View(createProductVM);
            }

            if (createProductVM.HoverImage.ValidSize(FileSize.MB, 3))
            {
                ModelState.AddModelError("HoverImage", "The image size must be less than 3 MB.");
                return View(createProductVM);
            }


            bool existsCategory = await _context.Categories.AnyAsync(c => c.Id == createProductVM.CategoryId);

            if (!existsCategory)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(createProductVM);
            }

            ProductImage mainImage = new ProductImage
            {
                ImageUrl = await createProductVM.MainImage.CreateFileAsync(_env.WebRootPath,"assets","images","website-images"),
                IsPrimary = true
            };

            ProductImage HiverImage = new ProductImage
            {
                ImageUrl = await createProductVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = false
            };

            Product product = new Product
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU,
                CategoryId = createProductVM.CategoryId,


            };

            if (createProductVM.AdditionalImage is not null)
            {
                string text= string.Empty;

                foreach(IFormFile file in createProductVM.AdditionalImage)
                {
                    if (!file.IsImage())
                    {
                        text+= $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }
                    
                    if (file.ValidSize(FileSize.MB, 3))
                    {
                        text += $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        IsPrimary = false
                    });

                }
                TempData["AdditionalImageError"] = text;
            }



           

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
    }
}
