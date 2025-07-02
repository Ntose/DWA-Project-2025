using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Data.Entities;

namespace WebAPI.Data
{
	public class HeritageDbContext : DbContext
	{
		public HeritageDbContext(DbContextOptions<HeritageDbContext> options)
			: base(options)
		{
		}

		public DbSet<NationalMinority> NationalMinority { get; set; }
		public DbSet<Topic> Topic { get; set; }
		public DbSet<CulturalHeritage> CulturalHeritage { get; set; }
		public DbSet<CulturalHeritageTopic> CulturalHeritageTopic { get; set; }
		public DbSet<ApplicationUser> ApplicationUser { get; set; }
		public DbSet<Comment> Comment { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// ---- Unique indexes ----
			modelBuilder.Entity<NationalMinority>()
				.HasIndex(n => n.Name).IsUnique();

			modelBuilder.Entity<Topic>()
				.HasIndex(t => t.Name).IsUnique();

			modelBuilder.Entity<CulturalHeritage>()
				.HasIndex(ch => ch.Name).IsUnique();

			modelBuilder.Entity<ApplicationUser>()
				.HasIndex(u => u.Username).IsUnique();

			modelBuilder.Entity<ApplicationUser>()
				.HasIndex(u => u.Email).IsUnique();

			// ---- Configure Role property default ----
			modelBuilder.Entity<ApplicationUser>()
				.Property(u => u.Role)
				.HasDefaultValue("User");  // all new users default to "User"

			// ---- Relationships ----
			modelBuilder.Entity<CulturalHeritage>()
				.HasOne(ch => ch.NationalMinority)
				.WithMany(nm => nm.CulturalHeritages)
				.HasForeignKey(ch => ch.NationalMinorityId);

			modelBuilder.Entity<CulturalHeritageTopic>()
				.HasKey(ct => new { ct.CulturalHeritageId, ct.TopicId });

			modelBuilder.Entity<CulturalHeritageTopic>()
				.HasOne(ct => ct.CulturalHeritage)
				.WithMany(ch => ch.CulturalHeritageTopics)
				.HasForeignKey(ct => ct.CulturalHeritageId);

			modelBuilder.Entity<CulturalHeritageTopic>()
				.HasOne(ct => ct.Topic)
				.WithMany(t => t.CulturalHeritageTopics)
				.HasForeignKey(ct => ct.TopicId);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.CulturalHeritage)
				.WithMany(ch => ch.Comments)
				.HasForeignKey(c => c.CulturalHeritageId);

			modelBuilder.Entity<Comment>()
				.HasOne(c => c.ApplicationUser)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId);

			// ---- Seed an initial Admin user ----
			modelBuilder.Entity<ApplicationUser>().HasData(
				new ApplicationUser
				{
					Id = 1,                      // pick an unused ID
					Username = "admin",
					Email = "admin@example.com",
					PasswordHash = "admin",                // plain-text for demo
					FirstName = "System",
					LastName = "Administrator",
					Phone = "000-000-0000",
					DateRegistered = DateTime.UtcNow,
					Role = "Admin"
				}
			);
		}
	}
}
