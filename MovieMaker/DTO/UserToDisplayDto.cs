using System.ComponentModel.DataAnnotations;

namespace Movies.DTO
{
    public class UserToDisplayDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}