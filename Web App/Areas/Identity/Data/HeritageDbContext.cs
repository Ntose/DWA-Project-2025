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

        // Entity sets representing database tables
        public DbSet<NationalMinority> NationalMinorities { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<CulturalHeritage> CulturalHeritages { get; set; }
        public DbSet<CulturalHeritageTopic> CulturalHeritageTopics { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply default Identity configuration
            base.OnModelCreating(builder);

            // Configure many-to-many relationship between CulturalHeritage and Topic
            builder.Entity<CulturalHeritageTopic>()
                .HasKey(ct => new { ct.CulturalHeritageId, ct.TopicId }); // Composite primary key

            builder.Entity<CulturalHeritageTopic>()
                .HasOne(ct => ct.CulturalHeritage)                        // One CulturalHeritage
                .WithMany(ch => ch.CulturalHeritageTopics)               // has many join entries
                .HasForeignKey(ct => ct.CulturalHeritageId);             // FK to CulturalHeritage

            builder.Entity<CulturalHeritageTopic>()
                .HasOne(ct => ct.Topic)                                   // One Topic
                .WithMany(t => t.CulturalHeritageTopics)                 // has many join entries
                .HasForeignKey(ct => ct.TopicId);                        // FK to Topic
        }
    }
}
