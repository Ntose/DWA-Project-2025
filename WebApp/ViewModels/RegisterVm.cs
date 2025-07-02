using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class RegisterVm
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
