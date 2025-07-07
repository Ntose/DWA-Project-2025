// ViewModel for displaying log entries in the admin interface
using System;

namespace WebApp.Models
{
    public class LogViewModel
    {
        public int Id { get; set; }                  // Unique log entry ID
        public string Level { get; set; } = "";      // Log severity (e.g. Info, Warning, Error)
        public string Message { get; set; } = "";    // Log message content
        public DateTime Timestamp { get; set; }      // When the log was recorded
    }
}
