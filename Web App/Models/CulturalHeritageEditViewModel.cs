// File: WebApp/Models/CulturalHeritageEditViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CulturalHeritageEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = "";

        [Display(Name = "Minority")]
        public int NationalMinorityId { get; set; }

        [Display(Name = "Themes")]
        public List<int> TopicIds { get; set; } = new();

        // lookup lists for dropdown/checkboxes
        public List<NationalMinorityViewModel> Minorities { get; set; } = new();
        public List<TopicViewModel> Topics { get; set; } = new();
    }
}
