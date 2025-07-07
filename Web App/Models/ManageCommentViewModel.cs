// ViewModel for managing user comments in the admin panel
using System;

namespace WebApp.Models
{
    public class ManageCommentViewModel
    {
        public int Id { get; set; }                        // Unique comment ID
        public int CulturalHeritageId { get; set; }        // Associated heritage item ID
        public string Text { get; set; } = "";             // Comment content
        public DateTime Timestamp { get; set; }            // When the comment was posted
        public bool Approved { get; set; }                 // Whether the comment is approved for display
    }
}
