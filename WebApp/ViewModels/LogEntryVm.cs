using System;

namespace WebApp.ViewModels
{
    public class LogEntryVm
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }      // e.g., "Info", "Warning", "Error"
        public string Message { get; set; }
        public string Source { get; set; }     // Optional (class/method)
    }
}
