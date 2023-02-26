using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;

namespace MoviesAPI.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().Property(obj => obj.Name).HasMaxLength(100);
            modelBuilder.Entity<Genre>().Property(obj => obj.Id).ValueGeneratedOnAdd();
            // modelBuilder.Entity<CreateGenreDto>().Property(obj=>obj.Name).HasMaxLength(100)
            modelBuilder.Entity<Movie>().Property(obj => obj.Title).HasMaxLength(250);
            modelBuilder.Entity<Movie>().Property(obj => obj.StoryLine).HasMaxLength(2500);
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
