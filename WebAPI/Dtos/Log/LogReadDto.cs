using System;

namespace WebAPI.Dtos.Log
{
	public class LogReadDto
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string Level { get; set; }
		public string Message { get; set; }
	}
}
