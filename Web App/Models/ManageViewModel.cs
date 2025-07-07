// ViewModel for displaying and managing a user's profile and comments
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class ManageViewModel
    {
        public string Username { get; set; } = "";         // User's login name
        public string Email { get; set; } = "";            // Email address
        public string FirstName { get; set; } = "";        // First name
        public string LastName { get; set; } = "";         // Last name
        public string Phone { get; set; } = "";            // Phone number
        public DateTime DateRegistered { get; set; }       // Account creation date
        public string Role { get; set; } = "";             // User role (e.g. Admin, User)

        // List of comments made by the user
        public List<ManageCommentViewModel> Comments { get; set; } = new();
    }
}
