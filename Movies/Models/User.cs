using Microsoft.AspNetCore.Identity;
using MovieMaker.Models;

namespace Movies.Models
{
    public class User : IdentityUser
    {
        public ICollection<Movie> Movies { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}
