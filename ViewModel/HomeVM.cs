using Pronia.Entities;

namespace Pronia.ViewModel
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Product>? Products { get; set; }
        public List<ProductImage>? ProductsImages { get; set; }
    }
}
