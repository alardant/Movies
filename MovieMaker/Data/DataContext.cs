using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieMaker.Models;
using Movies.Models;

namespace MovieMaker.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
                : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }

        
    }

}

