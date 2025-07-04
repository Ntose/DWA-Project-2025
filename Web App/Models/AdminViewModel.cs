// File: WebApp/Models/AdminViewModel.cs

using System.Collections.Generic;

namespace WebApp.Models
{
    public class AdminViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
        public int LogCount { get; set; }
        public List<LogViewModel> Logs { get; set; } = new();
    }
}
