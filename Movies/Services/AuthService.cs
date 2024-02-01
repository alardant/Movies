using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieMaker.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MovieMaker.DTO;
using UsersMovieMaker.DTO;
using MovieMaker.Models;
using MovieMaker.Repository;

namespace MovieMaker.Services
{
    public class AuthService
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration config, UserRepository userRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="userDto">the user data</param>
        /// <returns>True if the user has logged in; otherwise, false.</returns>
        public async Task<bool> Login(UserDto userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.Username);

            if (user == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(user, userDto.Password);
        }

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// /// /// <param name="password">The user password.</param>
        /// <returns>True if the user is created successfully; otherwise, false.</returns>
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var isUserAdmin = user.IsUserAdmin;

            if (isUserAdmin)
            {
                await AssignRolesAsync(user, isUserAdmin);

            }

            await _userManager.CreateAsync(user, password);
            return _userRepository.Save();
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

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtKey").Value));
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

