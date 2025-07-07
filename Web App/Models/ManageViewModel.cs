using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class ManageViewModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime DateRegistered { get; set; }
        public string Role { get; set; } = "";

        // User’s own comments
        public List<ManageCommentViewModel> Comments { get; set; } = new();
    }
}
