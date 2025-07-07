// ViewModel for submitting a new comment
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class CommentCreateViewModel
    {
        [Required, MinLength(1)]
        public string Text { get; set; } = ""; // Comment content (required)
    }
}
