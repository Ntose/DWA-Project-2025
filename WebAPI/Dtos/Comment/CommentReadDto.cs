using System;

namespace WebAPI.Dtos.Comment
{
    /// <summary>
    /// DTO used to send comment data to clients.
    /// Includes the comment text, timestamp, and author's username.
    /// </summary>
    public class CommentReadDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}
