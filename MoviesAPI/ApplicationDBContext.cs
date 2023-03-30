using Microsoft.EntityFrameworkCore;
using MoviesAPI.Entities;
using System.Diagnostics.CodeAnalysis;
namespace MoviesAPI
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext([NotNullAttribute] DbContextOptions options):base(options) 
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesActors>().HasKey(x => new { x.ActorId, x.MovieId });
            modelBuilder.Entity<MoviesGenres>().HasKey(x => new { x.GenreId, x.MovieId });
            modelBuilder.Entity<MoviesTheaters>().HasKey(x => new { x.TheaterId, x.MovieId });

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesActors> MoviesActors { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<MoviesTheaters> MoviesTheaters { get; set; }


    }
}
