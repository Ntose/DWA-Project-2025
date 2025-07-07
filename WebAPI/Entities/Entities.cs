using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Entities
{
    public class NationalMinority
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<CulturalHeritage> CulturalHeritages { get; set; } = new List<CulturalHeritage>();
    }

    public class Topic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<CulturalHeritageTopic> CulturalHeritageTopics { get; set; } = new List<CulturalHeritageTopic>();
    }

    public class CulturalHeritage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; }

        [Required]
        public int NationalMinorityId { get; set; }
        public NationalMinority NationalMinority { get; set; } = null!;

        public ICollection<CulturalHeritageTopic> CulturalHeritageTopics { get; set; } = new List<CulturalHeritageTopic>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class CulturalHeritageTopic
    {
        public int CulturalHeritageId { get; set; }
        public CulturalHeritage CulturalHeritage { get; set; } = null!;

        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
    }

    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        // NOTE: Plain-text for demo; use a hashed password in production.
        [Required]
        [StringLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;

        public DateTime DateRegistered { get; set; }

        /// <summary>
        /// The user's role. Allowed values: "User", "Admin".
        /// Defaults to "User" when registering new accounts.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        /// <summary>
        /// Navigation to the comments this user has posted.
        /// </summary>
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public bool Approved { get; set; }

        [Required]
        public int CulturalHeritageId { get; set; }
        public CulturalHeritage CulturalHeritage { get; set; } = null!;

        [Required]
        public int UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }

    /// <summary>
    /// Records each CRUD action (and errors if you choose) for auditing.
    /// </summary>
    public class Log
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Level { get; set; } = string.Empty; // e.g. "Info", "Error"

        [Required]
        public string Message { get; set; } = string.Empty;
    }

    // Optional: Legacy or simplified user model (not used in current logic)
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime DateRegistered { get; set; }
        public string Role { get; set; } = "User";
    }
}
