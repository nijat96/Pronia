using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ProniaAdminPanel.ViewModels.Category
{
    public class CreateCategoryVM
    {
        [Required(ErrorMessage ="Bos ola bilmez")]
        [MaxLength(20, ErrorMessage ="Uzunluq max 20 olmalidiir")]
        public string? Name { get; set; }
    }
}
