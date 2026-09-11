
using Pronia.Areas.ProniaAdminPanel.ViewModels.ProductImage;

namespace Pronia.Areas.ProniaAdminPanel.ViewModels.Product
{
    public class CreateProductVM
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? SKU { get; set; }
        public int CategoryId { get; set; }
        public List<Pronia.Entities.Category>? Categories { get; set; }
        public IFormFile? PrimaryImage { get; set; }
        public IFormFile? HoverImage { get; set; }
        public List<IFormFile>? AdditionalImages { get; set; }


    }
}
