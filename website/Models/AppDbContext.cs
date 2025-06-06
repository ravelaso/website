using Microsoft.EntityFrameworkCore;

namespace website.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<MusicProject> MusicProjects { get; set; }
        public DbSet<CodeProject> CodeProjects { get; set; }
        public DbSet<AboutEntry> AboutEntries { get; set; }
        public DbSet<AllowedUser> AllowedUsers { get; set; }
        public DbSet<ImageData> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/data.db");
        }
    }
}