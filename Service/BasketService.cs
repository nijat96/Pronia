using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.ViewModel.Basket;
using System.Security.Claims;

namespace Pronia.Service
{
    public class BasketService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public BasketService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }


        public async Task<List<BasketItemVM>> GetBasketAsync()
        {
            var basketList = new List<BasketItemVM>();

            var httpContext = _httpContext.HttpContext;

            if (httpContext == null) return basketList;

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                string? userId = httpContext.User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    var dbItems = await _context.BasketItems
                        .Where(b => b.AppUserId == userId)
                        .Include(b => b.Product)
                        .ThenInclude(p => p.ProductImages)
                        .ToListAsync();


                    foreach (var item in dbItems)
                    {
                        if (item.Product != null && !item.Product.IsDeleted)
                        {
                            string imageUrl = item.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl ?? "no-image.jpg";

                            basketList.Add(new BasketItemVM
                            {
                                Id = item.Id,
                                Name = item.Product.Name,
                                Price = item.Product.Price,
                                ImageUrl = imageUrl,
                                Count = item.Count,
                            });

                        }

                    }


                }
                else
                {
                    string? cookie = httpContext.Request.Cookies["basket"];

                    if (!string.IsNullOrEmpty(cookie))
                    {
                        var cookieItems = System.Text.Json.JsonSerializer.Deserialize<List<BasketCookieItemVM>>(cookie);
                        if (cookieItems != null && cookieItems.Count > 0)
                        {
                            foreach (var item in cookieItems)
                            {
                                var product = await _context.Products
                                    .Include(p => p.ProductImages)
                                    .FirstOrDefaultAsync(p => p.Id == item.Id && !p.IsDeleted);
                                if (product != null)
                                {
                                    string imageUrl = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.ImageUrl ?? "no-image.jpg";
                                    basketList.Add(new BasketItemVM
                                    {
                                        Id = product.Id,
                                        Name = product.Name,
                                        Price = product.Price,
                                        ImageUrl = imageUrl,
                                        Count = item.Count,
                                    });
                                }
                            }
                        }
                    }
                }

            }

            return basketList;
        }

        public async Task<int> GetBasketCountAsync()
        {
            var basketItems = await GetBasketAsync();
            return basketItems.Sum(item => item.Count);

        }
    }
}
