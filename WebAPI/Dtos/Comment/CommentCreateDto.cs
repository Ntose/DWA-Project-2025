using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Comment
{
	/// <summary>
	/// Used to receive create payload for a new comment.
	/// Client provides Text, CulturalHeritageId, and UserId.
	/// </summary>
	public class CommentCreateDto
	{
		[Required]
		public string Text { get; set; }

		[Required]
		public int CulturalHeritageId { get; set; }

		[Required]
		public int UserId { get; set; }
	}
}
