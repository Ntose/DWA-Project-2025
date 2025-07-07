using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Comment
{
    /// <summary>
    /// DTO used to receive data for creating a new comment.
    /// Client must provide Text, CulturalHeritageId, and UserId.
    /// </summary>
    public class CommentCreateDto
    {
        [Required(ErrorMessage = "Comment text is required.")]
        public string Text { get; set; } = string.Empty;

        [Required(ErrorMessage = "CulturalHeritageId is required.")]
        public int CulturalHeritageId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }
    }
}
