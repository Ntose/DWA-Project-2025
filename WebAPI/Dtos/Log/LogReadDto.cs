using System;

namespace WebAPI.Dtos.Log
{
    /// <summary>
    /// DTO used to send log entry data to clients.
    /// </summary>
    public class LogReadDto
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
