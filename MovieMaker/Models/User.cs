using Microsoft.AspNetCore.Identity;
using MovieMaker.Models;

namespace MovieMaker.Models
{
    public class User : IdentityUser
    {
        private ICollection<Movie> _movies { get; set; }

        public ICollection<Movie> Movies { get { return _movies; } set { _movies = value; } }
    }
}
