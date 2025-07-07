using System.Collections.Generic;
using Web_App.Models;

namespace WebApp.Models
{
    public class CulturalHeritageDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string MinorityName { get; set; } = "";
        public IEnumerable<string> Themes { get; set; }
            = new List<string>();

        public IEnumerable<CommentViewModel> Comments
        { get; set; } = new List<CommentViewModel>();

        public CommentCreateViewModel NewComment
        { get; set; } = new CommentCreateViewModel();
    }
}
