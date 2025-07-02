using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class UserProfileVm
    {
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
