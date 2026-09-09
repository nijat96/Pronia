using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ProniaAdminPanel.ViewModels.Category
{
    public class UpdateCategoryVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bos ola bilmez")]
        [MaxLength(20, ErrorMessage = "Uzunluq max 20 olmalidiir")]
        public string? Name { get; set; }
    }
}
