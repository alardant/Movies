using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMaker.DTO;
using MovieMaker.Models;
using MovieMaker.Repository;
using MovieMaker.Services;
using Serilog.Filters;
using System.Globalization;
using System.Security.Claims;

namespace Movies.Controllers
{
    /// <summary>
    /// Controller for managing movie-related operations.
    /// </summary>
    public class MovieController : ControllerBase
    {
        private readonly MovieRepository _movieRepository;
        private readonly UserRepository _userRepository;
        private readonly MovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        /// <param name="movieRepository">The repository for managing movie-related data.</param>
        /// <param name="userRepository">The repository for managing user-related data.</param>
        /// <param name="logger">The logger for capturing and logging controller-related events.</param>
        /// /// <param name="movieService">The service for managing movie-related operations.</param>
        public MovieController(MovieRepository movieRepository, UserRepository userRepository, ILogger<MovieController> logger, MovieService movieService)
        {
            _movieRepository = movieRepository;
            _logger = logger;
            _userRepository = userRepository;
            _movieService = movieService;
        }

        /// <summary>
        /// Gets all movies.
        /// </summary>
        /// <returns>List of movies.</returns>
        [HttpGet("Movies")]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _movieRepository.GetAllMoviesAsync();
                var moviesDto = new List<MovieDto>();
                foreach (Movie movie in movies)
                {
                    var movieDto = _movieService.ConvertMovieToMovieDto(movie);
                    moviesDto.Add(movieDto);
                }
                return Ok(moviesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching movies.");
            }

        }

        /// <summary>
        /// Gets a movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie.</param>
        /// <returns>The movie with the specified ID.</returns>
        [HttpGet("Movie/{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    _logger.LogError($"Movie with ID {id} not found.");
                    return NotFound($"An error occurred while fetching the movie.");
                }
                var movieDto = _movieService.ConvertMovieToMovieDto(movie);
                return Ok(movieDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the movie.");
            }
        }

        /// <summary>
        /// Filters movies based on a search string.
        /// </summary>
        /// <param name="str">The string used to research.</param>
        /// <returns>The filtered list of movies.</returns>
        [HttpGet("FilteredReviews")]
        public async Task<IActionResult> FilterMovies(string str)
        {
            try
            {
                var allMovies = await _movieRepository.GetAllMoviesAsync();
                var filteredMovies = allMovies
                .Where(i => i.Title.ToLower().Contains(str.ToLower()) ||
                    i.Description.ToLower().Contains(str.ToLower()) ||
                    Enum.GetName(typeof(MovieGenre), i.Genre)?.ToLower().Contains(str.ToLower()) == true ||
                    i.Author.ToLower().Contains(str.ToLower()))
                .ToList();

                var filteredmoviesDto = new List<MovieDto>();

                foreach (Movie filteredMovie in filteredMovies)
                {
                    var movieDto = _movieService.ConvertMovieToMovieDto(filteredMovie);
                    filteredmoviesDto.Add(movieDto);
                }

                return Ok(filteredmoviesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving movies.");
            }
        }

        /// <summary>
        /// Creates a new movie.
        /// </summary>
        /// <param name="movieDto">The movie data.</param>
        /// <returns>The newly created movie.</returns>
        [Authorize]
        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateMovie(MovieDto movieDto)
        {
            // Get the currently logged-in user's info
            var LoggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedUser = await _userRepository.GetUserByIdAsync(LoggedUserId);

            if (LoggedUserId == null || loggedUser == null)
            {
                return BadRequest("Échec de la création du commentaire.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Échec de la création du commentaire.");
            }

            try
            {
                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Description = movieDto.Description,
                    Author = movieDto.Author,
                    Genre = Enum.Parse<MovieGenre>(movieDto.Genre), // parse the value to convert them to appropriate types
                    DateOfRelease = DateTime.ParseExact(movieDto.DateOfRelease, "dd-MM-yyyy", CultureInfo.InvariantCulture), // parse the value to convert them to appropriate types
                    UserId = LoggedUserId,
                    User = loggedUser
                };

                var isMovieCreated = await _movieRepository.CreateMovieAsync(movie);

                if (!isMovieCreated)
                {
                    _logger.LogError($"An error occurred during creating data");
                    return StatusCode(500, "An error occurred while creating the movie.");
                }

                var MovieDtoCreated = _movieService.ConvertMovieToMovieDto(movie);
                return Ok(MovieDtoCreated);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during creating data: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the movie.");
            }
        }

        /// <summary>
        /// Updates an existing movie.
        /// </summary>
        /// <param name="id">The ID of the movie to update.</param>
        /// <param name="movieDto">The updated movie data.</param>
        /// <returns>The updated movie.</returns>
        [Authorize]
        [HttpPut("UpdateReview/{id}")]
        public async Task<IActionResult> UpdateMovie(int id, MovieDto movieDto)
        {
            if (!await _movieRepository.DoesMovieExists(id))
            {
                return NotFound("an error occurred while updating the movie.");
            }

            var movieToUpdate = await _movieRepository.GetMovieByIdAsync(id);
            var LoggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (LoggedUserId == null || LoggedUserId != movieToUpdate.UserId)
            {
                return BadRequest("An error occurred while updating the movie.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("An error occurred while updating the movie.");
            }

            try
            {
                var movie = await _movieRepository.GetMovieByIdAsync(id);
                movie.Title = movieDto.Title;
                movie.Description = movieDto.Description;
                movie.Author = movieDto.Author;
                movie.Genre = Enum.Parse<MovieGenre>(movieDto.Genre); // parse the value to convert them to appropriate types
                movie.DateOfRelease = DateTime.ParseExact(movieDto.DateOfRelease, "dd-MM-yyyy", CultureInfo.InvariantCulture); // parse the value to convert them to appropriate types

                var isMovieUpdated = await _movieRepository.UpdateMovieAsync(movie);

                if (!isMovieUpdated)
                {
                    _logger.LogError($"An error occurred during updating data");
                    return StatusCode(500, "An error occurred while updating the movie.");
                }
                var movieDtoUpdated = _movieService.ConvertMovieToMovieDto(movie);
                return Ok(movieDtoUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during updating data: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the movie.");
            }
        }

        /// <summary>
        /// Deletes a movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>True if the movie is deleted; false otherwise</returns>
        [Authorize]
        [HttpDelete("DeleteMovie")]
        public async Task<IActionResult> DeleteMovie(int id)
        {

            if (!await _movieRepository.DoesMovieExists(id)) 
            {
                return NotFound("An error occurred while deleting the movie.");
            }

            var movieToDelete = await _movieRepository.GetMovieByIdAsync(id);
            var LoggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (LoggedUserId == null || LoggedUserId != movieToDelete.UserId)
            {
                return BadRequest("Échec de la suppression du commentaire.");
            }

            try
            {
                var isMovieDeleted = await _movieRepository.DeleteMovieAsync(id);

                if (!isMovieDeleted)
                {
                    _logger.LogError($"An error occurred during deleting data");
                    return StatusCode(500, "An error occurred while deleting the movie.");
                }

                return Ok(isMovieDeleted);

            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred during deleting data: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the movie.");
            }
        }
    }
}
