using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Data.Entities;

namespace WebAPI.Data
{
	/// <summary>
	/// The EF Core DbContext for the Cultural Heritage application.
	/// Includes DbSets for all entities and configures indexes, defaults, relationships, seeding, and logging.
	/// </summary>
	public class HeritageDbContext : DbContext
	{
		public DbSet<CulturalHeritage> CulturalHeritages { get; set; }
		public HeritageDbContext(DbContextOptions<HeritageDbContext> options)
			: base(options)
		{
		}

		// Primary “lookup” and bridge tables
		public DbSet<NationalMinority> NationalMinority { get; set; }
		public DbSet<Topic> Topic { get; set; }
		public DbSet<CulturalHeritage> CulturalHeritage { get; set; }
		public DbSet<CulturalHeritageTopic> CulturalHeritageTopic { get; set; }

		// User, comments, and logs
		public DbSet<ApplicationUser> ApplicationUser { get; set; }
		public DbSet<Comment> Comment { get; set; }
		public DbSet<Log> Log { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// -------- Unique Indexes --------
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

			// -------- Default Values --------
			modelBuilder.Entity<CulturalHeritage>()
						.Property(ch => ch.DateAdded)
						.HasDefaultValueSql("GETDATE()");

			modelBuilder.Entity<ApplicationUser>()
						.Property(u => u.DateRegistered)
						.HasDefaultValueSql("GETDATE()");
			modelBuilder.Entity<ApplicationUser>()
						.Property(u => u.Role)
						.HasDefaultValue("User");

			modelBuilder.Entity<Comment>()
						.Property(c => c.Timestamp)
						.HasDefaultValueSql("GETDATE()");
			modelBuilder.Entity<Comment>()
						.Property(c => c.Approved)
						.HasDefaultValue(false);

			modelBuilder.Entity<Log>()
						.Property(l => l.Timestamp)
						.HasDefaultValueSql("GETDATE()");

			// -------- Relationships --------
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

			// -------- Seed Data --------
			modelBuilder.Entity<ApplicationUser>().HasData(
				new ApplicationUser
				{
					Id = 1,
					Username = "admin",
					Email = "admin@example.com",
					PasswordHash = "admin",          // demo only; hash in production
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
