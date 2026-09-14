using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ProniaAdminPanel.ViewModels.Product
{
    public class UpdateProductVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a main image.")]
        public IFormFile? MainImage { get; set; }
        [Required(ErrorMessage = "Please select a hover image.")]
        public IFormFile? HoverImage { get; set; }
        public List<IFormFile>? AdditionalImage { get; set; }
        [Required(ErrorMessage = "Please enter a product name.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters long.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Please enter a price.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid price.")]
        public decimal Price { get; set; }
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Please enter a SKU.")]
        [MaxLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
        public string? SKU { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }
        public Pronia.Entities.Category? Category { get; set; }
        public List<Pronia.Entities.Category>? Categories { get; set; }
        public List<Pronia.Entities.ProductImage>? ProductImages { get; set; }
        public List<int>? DeletedImageIds { get; set; } = new List<int>();
    }
}
