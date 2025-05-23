using Microsoft.EntityFrameworkCore;
using MyMediaLibrary.Models;

namespace MyMediaLibrary.Data
{
    public class MediaLibraryContext : DbContext
    {
        public DbSet<MediaItem> MediaItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=mediaLibrary.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.Property(e => e.Rating)
                    .HasDefaultValue(0.0)
                    .IsRequired();

                entity.Property(e => e.IsVisited)
                    .HasDefaultValue(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });
        }
    }
}