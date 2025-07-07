// ViewModel for user registration form
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }  // Desired username

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }  // User's first name

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }  // User's last name

        [Required]
        [Phone]
        public string Phone { get; set; }  // Contact phone number

        [Required]
        [EmailAddress]
        public string Email { get; set; }  // Valid email address

        [Required]
        [StringLength(100, MinimumLength = 6,
             ErrorMessage = "Password must be 6–100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Password input

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }  // Confirmation field

        public string? ReturnUrl { get; set; }  // Optional redirect after registration
    }
}
