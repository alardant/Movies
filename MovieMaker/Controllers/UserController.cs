using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Services;
using Movies.DTO;
using Movies.Repository;
using System.Globalization;
using System.Security.Claims;
using Movies.Models;
using Movies.DTO;
using Movies.Models;
using Movies.Repository;
using AutoMapper;

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
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="signInManager">The <see cref="SignInManager{IdentityUser}"/> for handling user sign-in operations.</param>
        /// <param name="authService">The service providing authentication-related functionality.</param>
        /// <param name="logger">The logger for capturing and logging controller-related events.</param>
        /// <param name="userRepository">The repository for managing user-related data.</param>
        /// /// <param name="mapper">The mapper.</param>
        public UserController(SignInManager<IdentityUser> signInManager, AuthService authService, ILogger<UserController> logger, UserRepository userRepository, IMapper mapper)
        {
            _signInManager = signInManager;
            _authService = authService;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
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
                var usersToDisplayDto = _mapper.Map<List<UserToDisplayDto>>(users);
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
        public async Task<IActionResult> Login(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Unable to Login");
            }

            try
            {

                var user = _mapper.Map<User>(userDto);
                var result = await _authService.Login(user);

                if (!result)
                {
                    return Unauthorized("An error occurred while logging in");
                }

                var tokenString = await _authService.GenerateJwtTokenAsString(user);

                return Ok(tokenString);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging in: {ex.Message}");
                return StatusCode(500, "An error occurred while logging in.");
            }

        }

        /// <summary>
        /// Logout a user 
        /// </summary>
        /// /// <returns>A message to say that the user has been logged out</returns>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok("User successfully log out");

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging out: {ex.Message}");
                return StatusCode(500, "An error occurred while logging out.");
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto">The user data.</param>
        /// <returns>The created user dto to display and the Jwt token.</returns>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Échec de la création du commentaire.");
            }

            try
            {
                var user = new User
                {
                    UserName = userDto.Username,
                    Email = userDto.Email,
                    IsUserAdmin = userDto.IsUserAdmin
                };

                var password = userDto.Password;

                var iUserCreated = await _userRepository.CreateUserAsync(user, password);

                if (!iUserCreated)
                {
                    _logger.LogError($"An error occurred during creating data");
                    return StatusCode(500, "An error occurred while creating the user.");
                }

                var UserDtoToDisplayCreated = _mapper.Map<UserToDisplayDto>(user);
                var tokenString = await _authService.GenerateJwtTokenAsString(user);

                return Ok(new { User = UserDtoToDisplayCreated, Token = tokenString });

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during creating data: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the user.");
            }
        }

        /// <summary>
        /// Update a user 
        /// </summary>
        /// <param name="id">the Id of the user to update</param>
        /// /<param name="userDto">The user data updated</param>
        /// <returns>The updated user</returns>
        [Authorize]
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult> UpdateUser(string id, UserDto userDto)
        {
            if (!await _userRepository.DoesUserExists(id))
            {
                return NotFound("an error occurred while updating the user.");
            }

            var userToUpdate = await _userRepository.GetUserByIdAsync(id);
            var LoggedUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (LoggedUserId == null || LoggedUserId != userToUpdate.Id)
            {
                return Unauthorized("An error occurred while updating the user.");
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                user.UserName = userDto.Username;
                user.Email = userDto.Email;
                var passwordUpdated = userDto.Password;

                var isUserUpdated = await _userRepository.UpdateUserAsync(user, passwordUpdated);

                if (!isUserUpdated)
                {
                    _logger.LogError($"An error occurred while updating data");
                    return StatusCode(500, "An error occurred while updating the user.");
                }

                var UserToDisplayDtoUpdated = _mapper.Map<UserToDisplayDto>(user);
                return Ok(UserToDisplayDtoUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating data: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        /// <summary>
        /// Delete a user 
        /// </summary>
        /// <param name="id">the Id of the user to delete</param>
        /// <returns>True if the user is deleted; false otherwise</returns>
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("An error occurred while deleting the user.");
            }


            try
            {
                var result = await _userRepository.DeleteUserAsync(id);

                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the user.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting data: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }



    }
}