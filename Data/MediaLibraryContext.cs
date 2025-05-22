using Microsoft.EntityFrameworkCore;
using MyMediaLibrary.Models;

namespace MyMediaLibrary.Data
{
    public class MediaLibraryContext : DbContext
    {
        public DbSet<MediaItem> MediaItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = @"C:\Users\cectpa\source\repos\MyMediaLibrary\MyMediaLibrary\mediaLibrary.db";
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaItem>().HasKey(x => x.Id);
        }
    }
}
