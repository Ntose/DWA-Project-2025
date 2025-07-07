// ViewModel for displaying a comment in the UI
using System;

namespace WebApp.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }                  // Unique identifier
        public string Text { get; set; } = "";       // Comment content
        public DateTime Timestamp { get; set; }      // When the comment was posted
        public string Author { get; set; } = "";     // Username or display name of the commenter
    }
}
