using Microsoft.AspNetCore.Identity;

namespace MovieMaker.Models
{
    public class User : IdentityUser
    {
        public ICollection<Movie> Movies { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}
