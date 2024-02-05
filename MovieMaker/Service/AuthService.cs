using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Movies.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Movies.Models;
using Movies.Repository;
using Movies.Controllers;

namespace Movies.Services
{
    /// <summary>
    /// Service for managing authentication.
    /// </summary>
    public class AuthService
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> used for role management.</param>
        /// <param name="userManager">The <see cref="UserManager{TUser}"/> used for user management.</param>
        /// <param name="config">The <see cref="IConfiguration"/> used for configuration settings.</param>
        /// <param name="logger">The logger for capturing and logging controller-related events.</param>

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration config, ILogger<UserController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="user">the user data</param>
        /// <returns>True if the user has logged in; otherwise, false.</returns>
        public async Task<bool> Login(UserLoginDto userLoginDto)
        {
            var identityUser = await GetUserForLogin(userLoginDto);

            if (identityUser == null)
            {
                return false;
            }

            try
            {
                return await _userManager.CheckPasswordAsync(identityUser, userLoginDto.Password);
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it)
                _logger.LogError($"An error occurred during user login: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Find User from UserLoginDto
        /// </summary>
        /// <param name="userLoginDto">the user data</param>
        /// <returns>The user</returns>
        public async Task<User> GetUserForLogin(UserLoginDto userLoginDto)
        {
            return await _userManager.FindByNameAsync(userLoginDto.Username);
        }

        /// <summary>
        /// Generate a JwoToken as a string
        /// </summary>
        /// <param name="userDto">the user data</param>
        /// <returns>A Jwt token as a string</returns>
        public async Task<string> GenerateJwtTokenAsString(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var securityToken = new JwtSecurityToken(
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: signingCred
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        /// <summary>
        /// Create roles if they don't exist.
        /// </summary>
        public async Task CreateRolesAsync()
        {
            // Check if roles exist and create them if not
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        /// <summary>
        /// Asssign roles to user.
        /// </summary>
        /// /// <param name="user">the user data</param>
        /// /// <param name="isUserAdmin">A bool to indicate if the user is admin or not</param>
        public async Task AssignRolesAsync(User user, bool isUserAdmin)
        {
            await CreateRolesAsync();

            if (isUserAdmin)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
        }

    }

}