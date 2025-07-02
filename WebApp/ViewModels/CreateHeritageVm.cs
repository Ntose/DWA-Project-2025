using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class CreateHeritageVm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        [Display(Name = "National Minority")]
        public int NationalMinorityId { get; set; }

        [Display(Name = "Topics")]
        public List<int> TopicIds { get; set; } = new();
    }
}
