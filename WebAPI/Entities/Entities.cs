using System;
using System.Collections.Generic;

namespace WebAPI.Data.Entities
{
	public class NationalMinority
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public ICollection<CulturalHeritage> CulturalHeritages { get; set; } = new List<CulturalHeritage>();

	}

	public class Topic
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public ICollection<CulturalHeritageTopic> CulturalHeritageTopics { get; set; } = new List<CulturalHeritageTopic>();

	}

	public class CulturalHeritage
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public DateTime DateAdded { get; set; }

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
		public string Username { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public DateTime DateRegistered { get; set; }

		public ICollection<Comment> Comments { get; set; } = new List<Comment>();
	}

	public class Comment
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public DateTime Timestamp { get; set; }
		public bool Approved { get; set; }

		public int CulturalHeritageId { get; set; }
		public CulturalHeritage CulturalHeritage { get; set; }

		public int UserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }
	}
}
