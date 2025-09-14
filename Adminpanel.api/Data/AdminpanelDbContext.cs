using Adminpanel.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adminpanel.api.Data
{
    public class AdminpanelDbContext : DbContext
    {
        public AdminpanelDbContext(DbContextOptions<AdminpanelDbContext> options) : base(options) {}

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<Page> Pages => Set<Page>();
        public DbSet<ContentPlacement> ContentPlacements => Set<ContentPlacement>();
        public DbSet<user> Users => Set<user>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // BLOG
            b.Entity<BlogPost>(e =>
            {
                e.Property(x => x.Title).IsRequired().HasMaxLength(160);
                e.Property(x => x.Slug).IsRequired().HasMaxLength(160);
                e.HasIndex(x => x.Slug).IsUnique();
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.Excerpt).HasMaxLength(400);
                e.Property(x => x.ImageUrl).HasMaxLength(512);
                e.HasIndex(x => new { x.Status, x.CreatedAt });
            });

            // PAGE
            b.Entity<Page>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(160);
                e.Property(x => x.Slug).IsRequired().HasMaxLength(160);
                e.HasIndex(x => x.Slug).IsUnique();
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.RedirectUrl).HasMaxLength(512);
            });

            // CONTENT PLACEMENT
            b.Entity<ContentPlacement>(e =>
            {
                e.Property(x => x.Position).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.ContentType).HasConversion<string>().HasMaxLength(20);
                e.Property(x => x.PathPattern).IsRequired().HasMaxLength(255);
                e.Property(x => x.ExternalUrl).HasMaxLength(512);

                // Optional links (either Page or Blog or External)
                e.HasOne(x => x.Page)
                  .WithMany()
                  .HasForeignKey(x => x.PageId)
                  .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Blog)
                  .WithMany()
                  .HasForeignKey(x => x.BlogId)
                  .OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(x => new { x.Active, x.Position });
                e.HasIndex(x => x.PathPattern);
                e.HasIndex(x => new { x.StartAt, x.EndAt });
            });

            // USER
            b.Entity<user>(e =>
            {
                e.Property(x => x.Email).IsRequired().HasMaxLength(256);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
                e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            });

            base.OnModelCreating(b);
        }
    }
}
