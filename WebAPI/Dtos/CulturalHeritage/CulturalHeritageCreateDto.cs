using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CulturalHeritage
{
	/// <summary>
	/// Used to receive create payload for CulturalHeritage.
	/// Client provides Name, Description, ImageUrl, NationalMinorityId, and TopicIds.
	/// </summary>
	public class CulturalHeritageCreateDto
	{
		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		public string Description { get; set; }

		[StringLength(500)]
		public string ImageUrl { get; set; }

		[Required]
		public int NationalMinorityId { get; set; }

		/// <summary>
		/// List of Topic IDs to associate with this heritage.
		/// </summary>
		public List<int> TopicIds { get; set; } = new List<int>();
	}
}
