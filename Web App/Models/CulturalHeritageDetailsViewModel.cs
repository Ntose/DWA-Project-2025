// ViewModel for displaying detailed information about a cultural heritage item
using System.Collections.Generic;
using Web_App.Models;

namespace WebApp.Models
{
    public class CulturalHeritageDetailsViewModel
    {
        public int Id { get; set; }                          // Heritage ID
        public string Name { get; set; } = "";               // Heritage name
        public string Description { get; set; } = "";        // Description text
        public string ImageUrl { get; set; } = "";           // Optional image URL
        public string MinorityName { get; set; } = "";       // Associated minority group

        public IEnumerable<string> Themes { get; set; } =    // Related topics/themes
            new List<string>();

        public IEnumerable<CommentViewModel> Comments        // Approved comments
        { get; set; } = new List<CommentViewModel>();

        public CommentCreateViewModel NewComment             // For submitting a new comment
        { get; set; } = new CommentCreateViewModel();
    }
}
