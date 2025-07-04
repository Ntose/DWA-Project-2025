// File: WebApp/Models/CulturalHeritageDetailsViewModel.cs
using System.Collections.Generic;

namespace WebApp.Models
{
    public class CulturalHeritageDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string MinorityName { get; set; } = "";
        public IEnumerable<string> Topics { get; set; } = new List<string>();
    }
}
