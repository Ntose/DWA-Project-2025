// File: WebApp/Models/ChangePasswordViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "New password must be 6–100 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }
}
