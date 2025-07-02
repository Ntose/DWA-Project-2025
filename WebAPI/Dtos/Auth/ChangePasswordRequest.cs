using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Auth
{
	public class ChangePasswordRequest
	{
		[Required]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }
	}
}
