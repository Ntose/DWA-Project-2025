using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
    /// <summary>
    /// DTO for changing a user's password.
    /// </summary>
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty; // Current password

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty; // New password
    }
}
