﻿using Microsoft.AspNetCore.Identity;
using Movies.Models;

namespace Movies.Models
{
    public class User : IdentityUser
    {
        public ICollection<Movie> Movies { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}