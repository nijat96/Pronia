using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModel.Basket;
using System.Security.Claims;
using System.Text.Json;

namespace Pronia.Controllers
{
    public class BasketController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {

                    var dbBasketItems = await _context.BasketItems.Where(b=>b.AppUserId==userId)
                        .Include(b=>b.Product)
                        .ThenInclude(p=>p.ProductImages)
                        .ToListAsync();

                    foreach(var item in dbBasketItems)
                    {
                        if (item.Product != null && !item.Product.IsDeleted)
                        {
                            string image = item.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true && !pi.IsDeleted).ImageUrl??"no-image.jpg";

                            basketItemVMs.Add(new BasketItemVM
                            {
                                Id = item.ProductId,
                                Name = item.Product.Name,
                                Price = item.Product.Price,
                                ImageUrl = image,
                                Count = item.Count
                            });
                        }
                    }

                }
                else
                {
                    string? cookie = Request.Cookies["basket"];

                    if (!string.IsNullOrEmpty(cookie))
                    {

                        List<BasketCookieItemVM>? cookieItemVMs = System.Text.Json.JsonSerializer.Deserialize<List<BasketCookieItemVM>>(cookie);

                        if (cookieItemVMs != null && cookieItemVMs.Count > 0)
                        {
                            foreach(var item in cookieItemVMs)
                            {
                                Product? product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == item.Id && !p.IsDeleted);
                                string image = product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary == true).ImageUrl ?? "no-image.jpg";

                                basketItemVMs.Add(new BasketItemVM
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Price = product.Price,
                                    ImageUrl = image,
                                    Count = item.Count


                                });
                            }
                        }

                    }

                  
                }

             

            }

            return View(basketItemVMs);
        }

        public async Task<IActionResult> AddBasket(int id,int count = 1)
        {
            if (id <= 0) return BadRequest();

            Product? existProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (existProduct == null) return NotFound();

            int addCount = count > 0 ? count : 1;

            if (User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                BasketItem? existDbItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.AppUserId == userId && b.ProductId == id);

                if (existDbItem == null)
                {
                    existDbItem = new BasketItem
                    {
                        AppUserId = userId,
                        ProductId = id,
                        Count = addCount
                    };

                    await _context.BasketItems.AddAsync(existDbItem);
                }
                else
                {
                    existDbItem.Count += addCount;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> cookieItemVMs;

                string? cookie = Request.Cookies["basket"];

                if (string.IsNullOrEmpty(cookie))
                {
                    cookieItemVMs = new List<BasketCookieItemVM>();
                }
                else
                {
                    cookieItemVMs = JsonSerializer.Deserialize<List<BasketCookieItemVM>>(cookie) ?? new List<BasketCookieItemVM>();
                }

                BasketCookieItemVM? existCookieItem = cookieItemVMs.FirstOrDefault(b=>b.Id==id);
                if (existCookieItem == null)
                {
                    cookieItemVMs.Add(new BasketCookieItemVM
                    {
                        Id = id,
                        Count = addCount
                    });
                }
                else
                {
                    existCookieItem.Count += addCount;
                }


                string json = JsonSerializer.Serialize(cookieItemVMs);

                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(14),
                    HttpOnly = true,
                    IsEssential = true
                };

                Response.Cookies.Append("basket", json, cookieOptions);



            }
           

            string? returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }



    }
}
