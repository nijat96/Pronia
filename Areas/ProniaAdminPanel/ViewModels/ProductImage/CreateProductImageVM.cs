namespace Pronia.Areas.ProniaAdminPanel.ViewModels.ProductImage
{
    public class CreateProductImageVM
    {
        public IFormFile Image { get; set; }
        public bool IsPrimary { get; set; }
        public int ProductId { get; set; }
    }
}
