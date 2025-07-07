using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CulturalHeritage
{
    /// <summary>
    /// DTO used to receive data for updating an existing CulturalHeritage entry.
    /// Same structure as the Create DTO.
    /// </summary>
    public class CulturalHeritageUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(200, ErrorMessage = "Name must not exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "NationalMinorityId is required.")]
        public int NationalMinorityId { get; set; }

        /// <summary>
        /// List of Topic IDs to associate with this heritage.
        /// </summary>
        public List<int> TopicIds { get; set; } = new();
    }
}
