using System;
using System.Collections.Generic;
using WebAPI.Dtos.NationalMinority;
using WebAPI.Dtos.Topic;

namespace WebAPI.Dtos.CulturalHeritage
{
    /// <summary>
    /// DTO used to send CulturalHeritage data to clients.
    /// Includes related NationalMinority and Topic summaries.
    /// </summary>
    public class CulturalHeritageReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; }

        public NationalMinorityReadDto NationalMinority { get; set; } = new();

        public List<TopicReadDto> Topics { get; set; } = new();
    }
}
