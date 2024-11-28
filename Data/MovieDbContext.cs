using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieApp.Models;

namespace MovieApp.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

        public DbSet<MovieTable> MovieTables { get; set; }
    }

}
