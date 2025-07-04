// File: WebApp/Models/CommentViewModel.cs
using System;

namespace WebApp.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Approved { get; set; }
        public int CulturalHeritageId { get; set; }
    }
}
