using System.ComponentModel.DataAnnotations;

namespace Movies.DTO
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}