// File: WebApp/Models/CulturalHeritageListViewModel.cs
using System.Collections.Generic;

namespace WebApp.Models
{
    public class CulturalHeritageListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string MinorityName { get; set; } = "";
        public IEnumerable<string> Topics { get; set; } = new List<string>();
    }
}
