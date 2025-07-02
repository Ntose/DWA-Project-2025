using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.Topic
{
	/// <summary>
	/// Used to receive create/update payload for Topic.
	/// </summary>
	public class TopicCreateDto
	{
		[Required]
		[StringLength(100)]
		public string Name { get; set; }
	}
}
