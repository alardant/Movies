using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MovieMaker.Models;
using Movies.DTO;
using Movies.Models;
using Movies.Repository;
using Movies.Services;

namespace Movies.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations.
    /// </summary>
    public class UserController : ControllerBase
    {
        
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AuthService _authService;
        private readonly UserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="signInManager">The SignInManager for handling user sign-in operations.</param>
        /// <param name="authService">The service providing authentication-related functionality.</param>
        /// <param name="logger">The logger for capturing and logging controller-related events.</param>
        /// <param name="userRepository">The repository for managing user-related data.</param>
        public UserController (SignInManager<IdentityUser> signInManager, AuthService authService, ILogger<UserController> logger, UserRepository userRepository)
        {
            _signInManager = signInManager;
            _authService = authService;
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>List of users.</returns>
        [Authorize("Admin")]
        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                var usersToDisplayDto = new List<UserToDisplayDto>();
                foreach (User user in users)
                {
                    var userToDisplayDto = ConvertUserToUserToDisplayDto(user);
                    usersToDisplayDto.Add(userToDisplayDto);
                }
                return Ok(usersToDisplayDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during retrieving data: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching users.");
            }
        }

        /// <summary>
        /// Login a user 
        /// </summary>
        /// <param name="userDto">the user data</param>
        /// <returns>A Jwt token as a string</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login (UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Unable to Login");
            }

            try
            {
                var result = await _authService.Login(userDto);
                if (!result)
                {
                    return BadRequest("An error occurred while logging in");
                }
                var tokenString = await _authService.GenerateJwtTokenAsString(userDto);
                return Ok(tokenString);
            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging in: {ex.Message}");
                return StatusCode(500, "An error occurred while logging in.");
            }
            
        }

        /// <summary>
        /// Logout a user 
        /// </summary>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();

            } catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging out: {ex.Message}");
                return StatusCode(500, "An error occurred while logging out.");
            }
        }

        /// <summary>
        /// Converts a <see cref="User"/> object to a <see cref="UserToDisplayDto"/>.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The user to display DTO.</returns>
        private UserToDisplayDto ConvertUserToUserToDisplayDto(User user)
        {
            return new UserToDisplayDto
            {
                Username = user.UserName,
                Email = user.Email
            };
        }

    }
}
