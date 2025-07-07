using System;

namespace WebApp.Models
{
    public class ManageCommentViewModel
    {
        public int Id { get; set; }
        public int CulturalHeritageId { get; set; }
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public bool Approved { get; set; }
    }
}
