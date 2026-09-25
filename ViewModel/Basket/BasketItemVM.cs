using MimeKit.Tnef;

namespace Pronia.ViewModel.Basket
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public  int Count { get; set; }
        public  decimal SubTotal => Price * Count;
    }
}
