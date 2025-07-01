using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Entities;

namespace WebAPI.Data
{
	public class HeritageDbContext : DbContext
	{
		public HeritageDbContext(DbContextOptions<HeritageDbContext> options)
			: base(options)
		{
		}

		public DbSet<NationalMinority> NationalMinorities { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<CulturalHeritage> CulturalHeritages { get; set; }
		public DbSet<CulturalHeritageTopic> CulturalHeritageTopics { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Comment> Comments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// enforce unique names
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

			// Primary‐to‐many: CulturalHeritage → NationalMinority
			modelBuilder.Entity<CulturalHeritage>()
				.HasOne(ch => ch.NationalMinority)
				.WithMany(nm => nm.CulturalHeritages)
				.HasForeignKey(ch => ch.NationalMinorityId);

			// Many‐to‐many bridge: CulturalHeritageTopic
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

			// Comments → CulturalHeritage & ApplicationUser
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.CulturalHeritage)
				.WithMany(ch => ch.Comments)
				.HasForeignKey(c => c.CulturalHeritageId);
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.ApplicationUser)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
