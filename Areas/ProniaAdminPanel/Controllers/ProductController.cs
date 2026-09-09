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

            Product product = new Product
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU,
                CategoryId = createProductVM.CategoryId
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (!product.ProductImages.IsNullOrEmpty())
            {
                int productId = await _context.Products.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefaultAsync();

                List<ProductImage> productImages = new List<ProductImage>();
                
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "website-images");
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach (var imageVM in createProductVM.Images)
                {
                    string imageName = Path.GetFileNameWithoutExtension(imageVM.Image.FileName);
                    string uniqueImageName = $"{Guid.NewGuid()}_{imageName}";
                    string imageExtension = Path.GetExtension(imageVM.Image.FileName);
                    string newImageName = String.Concat(uniqueImageName, imageExtension);
                    var imagePath = Path.Combine(path, newImageName);

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await imageVM.Image.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }

                    ProductImage productImage = new ProductImage
                    {
                        IsPrimary = imageVM.IsPrimary,
                        ProductId = productId,
                        ImageUrl = newImageName
                    };
                    productImages.Add(productImage);
                }
                if (productImages.Count > 0 && !productImages.Any(p => p.IsPrimary == true))
                {
                    productImages.First().IsPrimary = true;
                }
                if (productImages.Count > 1 && !productImages.Any(p => p.IsPrimary == false))
                {
                    productImages.First(p => p.IsPrimary != true).IsPrimary = false;
                }
                await _context.ProductImages.AddRangeAsync(productImages);
                await _context.SaveChangesAsync();
            }

            return View("ss");
        }


    }
}
