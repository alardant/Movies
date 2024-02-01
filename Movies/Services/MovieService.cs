using MovieMaker.DTO;
using MovieMaker.Models;

namespace MovieMaker.Services
{
    public class MovieService
    {
        public MovieService() { }

        /// <summary>
        /// Converts a <see cref="Movie"/> object to a <see cref="MovieDto"/>.
        /// </summary>
        /// <param name="movie">The movie object.</param>
        /// <returns>The movie DTO.</returns>
        public MovieDto ConvertMovieToMovieDto(Movie movie)
        {
            return new MovieDto
            {
                Title = movie.Title,
                Description = movie.Description,
                Author = movie.Author,
                Genre = movie.Genre.ToString(),
                DateOfRelease = movie.DateOfRelease.ToString("dd-MM-yyyy")
            };
        }
    }
}
