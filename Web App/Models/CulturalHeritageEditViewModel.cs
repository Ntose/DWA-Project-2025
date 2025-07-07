// ViewModel for creating or editing a cultural heritage entry
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CulturalHeritageEditViewModel
    {
        public int Id { get; set; }  // Used for editing existing entries

        [Required]
        public string Name { get; set; } = "";  // Heritage name (required)

        public string Description { get; set; } = "";  // Optional description

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = "";  // Link to an image (required)

        [Display(Name = "Minority")]
        public int NationalMinorityId { get; set; }  // Selected minority group

        [Display(Name = "Themes")]
        public List<int> TopicIds { get; set; } = new();  // Selected topic IDs

        // Populated lists for form dropdowns and checkboxes
        public List<NationalMinorityViewModel> Minorities { get; set; } = new();
        public List<TopicViewModel> Topics { get; set; } = new();
    }
}
