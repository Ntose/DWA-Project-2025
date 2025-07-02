using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
	public class LoginRequest
	{
		[Required]
		[StringLength(100)]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
