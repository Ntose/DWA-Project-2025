using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Entities;

namespace WebAPI.Data
{
	public class HeritageDbContext : IdentityDbContext<IdentityUser>
	{
		public HeritageDbContext(DbContextOptions<HeritageDbContext> options)
			: base(options)
		{
		}

		public DbSet<NationalMinority> NationalMinorities { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<CulturalHeritage> CulturalHeritages { get; set; }
		public DbSet<CulturalHeritageTopic> CulturalHeritageTopics { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Log> Logs { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// configure composite key for the join table
			builder.Entity<CulturalHeritageTopic>()
				.HasKey(ct => new { ct.CulturalHeritageId, ct.TopicId });

			builder.Entity<CulturalHeritageTopic>()
				.HasOne(ct => ct.CulturalHeritage)
				.WithMany(ch => ch.CulturalHeritageTopics)
				.HasForeignKey(ct => ct.CulturalHeritageId);

			builder.Entity<CulturalHeritageTopic>()
				.HasOne(ct => ct.Topic)
				.WithMany(t => t.CulturalHeritageTopics)
				.HasForeignKey(ct => ct.TopicId);
		}
	}
}
