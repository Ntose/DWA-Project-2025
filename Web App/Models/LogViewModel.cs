// File: WebApp/Models/LogViewModel.cs
using System;

namespace WebApp.Models
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public string Level { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
