using Pronia.Entities;

namespace Pronia.ViewModel
{
    public class ProductVM
    {
        public Product? Product { get; set; }
        public List<Product>? RelatedProducts { get; set; }
    }
}
