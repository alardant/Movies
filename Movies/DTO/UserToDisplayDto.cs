using System.ComponentModel.DataAnnotations;

namespace MovieMaker.DTO
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
