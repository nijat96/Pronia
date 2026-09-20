using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModel.Autho
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
        public string Name { get; set; } = default!;
        [Required(ErrorMessage = "Surname is required")]
        [MinLength(2, ErrorMessage = "Surname must be at least 2 characters long")]
        public string Surname { get; set; } = default!;
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = default!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = default!;
    }
}
