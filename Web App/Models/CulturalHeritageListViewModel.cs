// ViewModel for listing cultural heritage items in summaries or tables
using System.Collections.Generic;

namespace WebApp.Models
{
    public class CulturalHeritageListViewModel
    {
        public int Id { get; set; }                        // Unique identifier
        public string Name { get; set; } = "";             // Heritage name
        public string MinorityName { get; set; } = "";     // Associated minority group
        public IEnumerable<string> Topics { get; set; } =  // Related topics/themes
            new List<string>();
    }
}
