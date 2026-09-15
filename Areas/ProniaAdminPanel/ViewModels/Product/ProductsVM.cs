using Pronia.Entities;

namespace Pronia.Areas.ProniaAdminPanel.ViewModels.Product
{
    public class ProductsVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }

        public string? CategoryName { get; set; }
        public string? MainImageUrl { get; set; }
        public bool IsDeleted { get; set; }
    }
}
