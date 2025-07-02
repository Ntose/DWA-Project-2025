using System;
using System.Collections.Generic;
using WebAPI.Dtos.NationalMinority;
using WebAPI.Dtos.Topic;

namespace WebAPI.Dtos.CulturalHeritage
{
	/// <summary>
	/// Used to send CulturalHeritage data to clients.
	/// Includes related NationalMinority and Topic summaries.
	/// </summary>
	public class CulturalHeritageReadDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string ImageUrl { get; set; }

		public DateTime DateAdded { get; set; }

		public NationalMinorityReadDto NationalMinority { get; set; }

		public List<TopicReadDto> Topics { get; set; }
	}
}
