using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CulturalHeritage
{
	/// <summary>
	/// Used to receive update payload for CulturalHeritage.
	/// Same shape as CreateDto.
	/// </summary>
	public class CulturalHeritageUpdateDto
	{
		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		public string Description { get; set; }

		[StringLength(500)]
		public string ImageUrl { get; set; }

		[Required]
		public int NationalMinorityId { get; set; }

		public List<int> TopicIds { get; set; } = new List<int>();
	}
}
