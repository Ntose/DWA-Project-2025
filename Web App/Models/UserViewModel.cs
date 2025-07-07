// ViewModel representing a user in the system (used in admin views)
using System;

namespace WebApp.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }                      // Unique user ID
        public string Username { get; set; } = "";       // Login name
        public string Email { get; set; } = "";          // Email address
        public string Role { get; set; } = "";           // Assigned role (e.g. Admin, User)
        public DateTime DateRegistered { get; set; }     // Account creation date
    }
}
