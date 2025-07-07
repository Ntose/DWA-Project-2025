using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
    /// <summary>
    /// DTO for user login credentials.
    /// </summary>
    public class LoginRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "Username must not exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
