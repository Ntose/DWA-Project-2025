using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
	public class RegisterRequest
	{
		[Required]
		[StringLength(100)]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(200)]
		public string Email { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[StringLength(100)]
		public string FirstName { get; set; }

		[StringLength(100)]
		public string LastName { get; set; }

		[Phone]
		[StringLength(50)]
		public string Phone { get; set; }
	}
}
