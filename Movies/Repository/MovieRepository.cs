using Microsoft.EntityFrameworkCore;
using MovieMaker.Models;
using Movies.Data;
using Movies.Models;

namespace Movies.Repository
{
    public class MovieRepository
    {
        private readonly DataContext _context;

        public MovieRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToArrayAsync();
        }

        public async Task<Movie> GetMoviesByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ICollection<Movie>> GetMoviesByUserAsync(string id)
        {
            return await _context.Movies.Where(i => i.UserId == id).ToListAsync();
        }

        public async Task<bool> CreateMovieAsync(Movie movie, User user)
        {
                movie.UserId = user.Id;
                movie.User = user;

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                return true;
        }
    }
}
