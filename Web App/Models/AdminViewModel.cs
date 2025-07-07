// Represents the data shown on the admin dashboard
using System.Collections.Generic;

namespace WebApp.Models
{
    public class AdminViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();   // List of all users
        public int LogCount { get; set; }                         // Total number of logs
        public List<LogViewModel> Logs { get; set; } = new();     // Recent log entries
    }
}
