using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.ProniaAdminPanel.ViewModels.Product;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.Utilities.Enum;
using Pronia.Utilities.Extensions;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class ProductController(AppDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.ProductImages).ToListAsync();
            var productsVM = products.Select(p => new ProductsVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                MainImageUrl = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl,
                IsDeleted = p.IsDeleted
            }).ToList();
            return View(productsVM);
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

            if (!createProductVM.MainImage.ValidSize(FileSize.MB, 3))
            {
                ModelState.AddModelError("MainImage", "The image size must be less than 3 MB.");
                return View(createProductVM);
            }

            if (!createProductVM.HoverImage.ValidSize(FileSize.MB, 3))
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

            Product product = new Product
            {
                Name = createProductVM.Name,
                Price = createProductVM.Price,
                Description = createProductVM.Description,
                SKU = createProductVM.SKU,
                CategoryId = createProductVM.CategoryId
            };

            string mainImagePath = await createProductVM.MainImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            ProductImage mainImage = new ProductImage
            {
                ImageUrl = mainImagePath,
                IsPrimary = true,
                ProductId = product.Id
            };
            string hoverImagePath = await createProductVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
            ProductImage HoverImage = new ProductImage
            {
                ImageUrl = hoverImagePath,
                IsPrimary = false,
                ProductId = product.Id
            };
            product.ProductImages ??= new List<ProductImage>();
            product.ProductImages.Add(mainImage);
            product.ProductImages.Add(HoverImage);

            if (createProductVM.AdditionalImage is not null)
            {
                string text = string.Empty;

                foreach (IFormFile file in createProductVM.AdditionalImage)
                {
                    if (!file.IsImage())
                    {
                        text += $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }

                    if (!file.ValidSize(FileSize.MB, 3))
                    {
                        text += $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }
                    string additionalImagePath = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                    ProductImage productImage = new ProductImage
                    {
                        ImageUrl = additionalImagePath,
                        IsPrimary = null,
                        ProductId = product.Id
                    };
                    product.ProductImages.Add(productImage);

                }
                TempData["AdditionalImageError"] = text;
            }





            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet("ProniaAdminPanel/Product/Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            Product product = await _context.Products.Include(c => c.Category).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            UpdateProductVM updateProductVM = new UpdateProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                Category = product.Category,
                Categories = await _context.Categories.ToListAsync(),
                ProductImages = product.ProductImages
            };

            return View(updateProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM, int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            Product product = await _context.Products.Include(p => p.ProductImages).Include(c => c.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            updateProductVM.Categories = await _context.Categories.ToListAsync();
            updateProductVM.ProductImages = product.ProductImages;
            if (!ModelState.IsValid)
            {
                return View(updateProductVM);
            }
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateProductVM.CategoryId);
            if (!categoryExists)
            {
                
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(updateProductVM);
            }

            

            product.Name = updateProductVM.Name;
            product.Price = updateProductVM.Price;
            product.Description = updateProductVM.Description;
            product.SKU = updateProductVM.SKU;
            product.CategoryId = updateProductVM.CategoryId;



            if (updateProductVM.MainImage is not null)
            {
                if (!updateProductVM.MainImage.IsImage())
                {
                    ModelState.AddModelError("MainImage", "Please select a valid image file.");
                    return View(updateProductVM);
                }
                if (!updateProductVM.MainImage.ValidSize(FileSize.MB, 3))
                {
                    ModelState.AddModelError("MainImage", "The image size must be less than 3 MB.");
                    return View(updateProductVM);
                }
                string mainImagePath = await updateProductVM.MainImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                product.ProductImages.FirstOrDefault(p => p.IsPrimary == true).ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                product.ProductImages.FirstOrDefault(p => p.IsPrimary == true).ImageUrl = mainImagePath;
            }
            if (updateProductVM.HoverImage is not null)
            {
                if (!updateProductVM.HoverImage.IsImage())
                {
                    ModelState.AddModelError("HoverImage", "Please select a valid image file.");
                    return View(updateProductVM);
                }
                if (!updateProductVM.HoverImage.ValidSize(FileSize.MB, 3))
                {
                    ModelState.AddModelError("HoverImage", "The image size must be less than 3 MB.");
                    return View(updateProductVM);
                }
                string hoverImagePath = await updateProductVM.HoverImage.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                product.ProductImages.FirstOrDefault(p => p.IsPrimary == false).ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                product.ProductImages.FirstOrDefault(p => p.IsPrimary == false).ImageUrl = hoverImagePath;
            }
            if (updateProductVM.AdditionalImage is not null)
            {
                string text = string.Empty;
                foreach (IFormFile file in updateProductVM.AdditionalImage)
                {
                    if (!file.IsImage())
                    {
                        text += $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }
                    if (!file.ValidSize(FileSize.MB, 3))
                    {
                        text += $"<p class=\"text-danger\">{file.FileName} is not a valid image file.</p>";
                        continue;
                    }
                    string additionalImagePath = await file.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = additionalImagePath,
                        IsPrimary = null,
                        ProductId = product.Id
                    });
                }
                TempData["AdditionalImageError"] = text;
            }

            if(updateProductVM.DeletedImageIds is not null)
            {
                foreach(var imageId in updateProductVM.DeletedImageIds)
                {
                    if(product.ProductImages.Any(i=> i.Id == imageId))
                    {
                        var image = product.ProductImages.FirstOrDefault(i=> i.Id == imageId);
                        image.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                        product.ProductImages.Remove(image);
                    }
                }
            }

            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
