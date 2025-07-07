using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Topic
{
    /// <summary>
    /// DTO used to receive create or update payload for a Topic.
    /// </summary>
    public class TopicCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
