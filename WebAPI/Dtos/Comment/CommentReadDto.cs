using System;

namespace WebAPI.Dtos.Comment
{
	/// <summary>
	/// Used to send comment data to clients.
	/// Includes the commenting user's username.
	/// </summary>
	public class CommentReadDto
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public DateTime Timestamp { get; set; }
		public string Username { get; set; }
	}
}
