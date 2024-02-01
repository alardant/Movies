using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Movies.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.Services
{
    public class AuthService
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }

        /// <summary>
        /// Create roles if they don't exist.
        /// </summary>
        /// <param name="userDto">the user data</param>
        /// <returns>A bool</returns>
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
        /// Generate a JwoToken as a string
        /// </summary>
        /// <param name="userDto">the user data</param>
        /// <returns>A Jwt token as a string</returns>
        public async Task<string> GenerateJwtTokenAsString (UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDto.Username),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtKey").Value));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var securityToken = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddMinutes(15),
               issuer: _config.GetSection("Jwt:Issuer").Value,
               audience: _config.GetSection("Jwt:Audience").Value,
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

    }

}

