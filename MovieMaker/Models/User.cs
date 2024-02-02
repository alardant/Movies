using Microsoft.AspNetCore.Identity;
using Movies.Models;

namespace Movies.Models
{
    public class User : IdentityUser
    {
        private ICollection<Movie> _movies { get; set; }

        public ICollection<Movie> Movies { get { return _movies; } set { _movies = value; } }
    }
}
