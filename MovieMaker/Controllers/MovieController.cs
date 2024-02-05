using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Models;
using Movies.Data;
using Movies.DTO;
using Movies.Repository;
using Serilog.Filters;
using System.Globalization;
using System.Security.Claims;
using AutoMapper;

namespace Movies.Controllers
{
    /// <summary>
    /// Controller for managing movie-related operations.
    /// </summary>
    public class MovieController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly UserRepository _userRepository;
        private readonly ILogger<MovieController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        /// <param name="movieRepository">The movie repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public MovieController(MovieRepository movieRepository, UserRepository userRepository, ILogger<MovieController> logger, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
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
                var moviesDto = _mapper.Map<List<MovieDto>>(movies);
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
                var movieDto = _mapper.Map<MovieDto>(movie);
                return Ok(movieDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the movie.");
            }
        }

        [Authorize]
        [HttpPost("Test")]
        public string Test()
        {
            return "Test";
        }

        /// <summary>
        /// Creates a new movie.
        /// </summary>
        /// <param name="movieDto">The movie data.</param>
        /// <returns>The newly created movie.</returns>
        [Authorize]
        [HttpPost("CreateMovie")]
        public async Task<IActionResult> CreateMovie(MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Échec de la création du commentaire.");
            }

            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (userId == null ||user == null)
                {
                    return BadRequest("User cannot be retrieved");
                }

                Movie movie = _mapper.Map<Movie>(movieDto);
                movie.UserId = userId;
                movie.User = user;
                movie.DateOfRelease = movieDto.DateOfRelease;

                var isMovieCreated = await _movieRepository.CreateMovieAsync(movie);

                if (!isMovieCreated)
                {
                    _logger.LogError($"An error occurred during creating data");
                    return StatusCode(500, "An error occurred while creating the movie.");
                }
                
                return Ok(movieDto);

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
        [HttpPut("UpdateMovie/{id}")]
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
                _mapper.Map(movieDto, movie);

                var isMovieUpdated = await _movieRepository.UpdateMovieAsync(movie);

                if (!isMovieUpdated)
                {
                    _logger.LogError($"An error occurred during updating data");
                    return StatusCode(500, "An error occurred while updating the movie.");
                }
                var updatedMovieDto = _mapper.Map<MovieDto>(movie);
                return Ok(updatedMovieDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during updating data: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the movie.");
            }
        }

        /// <summary>
        /// Filters movies based on a search string.
        /// </summary>
        /// <param name="str">The search string.</param>
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

                var filteredMoviesDto = _mapper.Map<List<MovieDto>>(filteredMovies);

                return Ok(filteredMoviesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving movies.");
            }
        }

        /// <summary>
        /// Deletes a movie by ID.
        /// </summary>
        /// <param name="id">The ID of the movie to delete.</param>
        /// <returns>True if the movie was deleted; false otherwise</returns>
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

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during deleting data: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the movie.");
            }
        }
    }
}
