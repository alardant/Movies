using Microsoft.EntityFrameworkCore;
using MovieMaker.Models;

namespace Movies.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
                : base(options)
        {
        }
        public DbSet<Movie> Mvoies { get; set; }

    }
}
