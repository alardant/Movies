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
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ICollection<Movie>> GetMoviesByUserAsync(string id)
        {
            return await _context.Movies.Where(i => i.UserId == id).ToListAsync();
        }

        public async Task<bool> CreateMovieAsync(Movie movie)
        {

            _context.Movies.Add(movie);
            return Save();
        }

        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return Save();
        }

        public async Task<bool> RemoveMovieAsync(int id)
        {
            var movie = _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
            _context.Remove(movie);
            _context.SaveChanges();
            return Save();
        }


        public async Task<bool> DoesMovieExists(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
            if (movie == null)
            {
                return false;
            }
            return true;

        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


    }
}
