﻿// File: WebApp/Models/UserViewModel.cs
using System;

namespace WebApp.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime DateRegistered { get; set; }
        public string Role { get; set; } = "";
    }
}
