using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMaker.Models;
using Movies.Data;
using Movies.DTO;
using Movies.Repository;
using System.Security.Claims;

namespace Movies.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly UserRepository _userRepository;
        private readonly ILogger<MovieController> _logger;
        public MovieController(MovieRepository movieRepository,UserRepository userRepository, ILogger<MovieController> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var movies = await _movieRepository.GetAllMoviesAsync();
                var movieDtos = movies.Select(movie => ConvertToMovieDto(movie)).ToList();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching reviews.");
            }

        }

        [HttpGet("id")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMoviesByIdAsync(id);
                if (movie == null)
                {
                    _logger.LogError($"Movie with ID {id} not found.");
                    return NotFound($"Movie with ID {id} not found.");
                }
                var movieDto = ConvertToMovieDto(movie);
                return Ok(movieDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the review.");
            }
        }

        private MovieDto ConvertToMovieDto(Movie movie)
        {
            return new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Author = movie.Author,
                Genre = movie.Genre.ToString(),
                DateOfRelease = movie.DateOfRelease.ToString("dd-MM-yyyy")
            };
        }
    }
}
