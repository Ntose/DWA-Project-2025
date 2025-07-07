using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
    /// <summary>
    /// DTO for user registration.
    /// </summary>
    public class RegisterRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Username must not exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200, ErrorMessage = "Email must not exceed 200 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "First name must not exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Last name must not exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [StringLength(50, ErrorMessage = "Phone number must not exceed 50 characters.")]
        public string Phone { get; set; } = string.Empty;
    }
}
