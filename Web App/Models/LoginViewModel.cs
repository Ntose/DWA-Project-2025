// ViewModel for user login form
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }  // Username or email (required)

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }  // Password input (required)

        public bool RememberMe { get; set; }  // Option to persist login

        public string? ReturnUrl { get; set; }  // Redirect target after login
    }
}
