﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Data.Entities
{
	public class NationalMinority
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		public ICollection<CulturalHeritage> CulturalHeritages { get; set; } = new List<CulturalHeritage>();
	}

	public class Topic
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		public ICollection<CulturalHeritageTopic> CulturalHeritageTopics { get; set; } = new List<CulturalHeritageTopic>();
	}

	public class CulturalHeritage
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		public string Description { get; set; }

		[StringLength(500)]
		public string ImageUrl { get; set; }

		public DateTime DateAdded { get; set; }

		[Required]
		public int NationalMinorityId { get; set; }
		public NationalMinority NationalMinority { get; set; }

		public ICollection<CulturalHeritageTopic> CulturalHeritageTopics { get; set; } = new List<CulturalHeritageTopic>();
		public ICollection<Comment> Comments { get; set; } = new List<Comment>();
	}

	public class CulturalHeritageTopic
	{
		public int CulturalHeritageId { get; set; }
		public CulturalHeritage CulturalHeritage { get; set; }

		public int TopicId { get; set; }
		public Topic Topic { get; set; }
	}

	public class ApplicationUser
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		[StringLength(200)]
		public string Email { get; set; }

		// NOTE: Plain-text for demo; swap for a hash in production.
		[Required]
		[StringLength(500)]
		public string PasswordHash { get; set; }

		[StringLength(100)]
		public string FirstName { get; set; }

		[StringLength(100)]
		public string LastName { get; set; }

		[Phone]
		[StringLength(50)]
		public string Phone { get; set; }

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
		public string Text { get; set; }

		public DateTime Timestamp { get; set; }

		public bool Approved { get; set; }

		[Required]
		public int CulturalHeritageId { get; set; }
		public CulturalHeritage CulturalHeritage { get; set; }

		[Required]
		public int UserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }
	}
	/// <summary>
	/// Records each CRUD action (and errors if you choose) for auditing.
	/// </summary>
	public class Log
	{
		public int Id { get; set; }

		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

		[Required, StringLength(20)]
		public string Level { get; set; }      // e.g. "Info", "Error"

		[Required]
		public string Message { get; set; }
	}
}
