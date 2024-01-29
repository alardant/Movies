using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MovieMaker.Models;
using Movies.Models;
using Microsoft.EntityFrameworkCore;

namespace Movies.Data
{
    public class SeedData
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedData(UserManager<User> userManager, DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _roleManager = roleManager;
        }

        public async Task SeedDataAsync()
        {
            if (!_dataContext.Movies.Any() && !_dataContext.Users.Any())
            {
                // Ensure role Admin exists
                var roleExists = await _roleManager.RoleExistsAsync("Admin");
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Seed Users
                var users = new[]
                {
                    new User
                    {
                        UserName = "User1",
                        Email = "user1@example.com",
                    },
                    new User
                    {
                        UserName = "User2",
                        Email = "user2@example.com",
                    },
                };

                foreach (var user in users)
                {
                    var userCreationResult = await _userManager.CreateAsync(user, "Password123!");
                    if (userCreationResult.Succeeded && user.UserName == "User1")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                }

                await _dataContext.SaveChangesAsync();

                // Get created users with their IDs
                var createdUsers = await _dataContext.Users.ToListAsync();

                // Seed Movies
                var movies = new[]
                {
                    new Movie
                    {
                        Title = "Movie 1",
                        Description = "Description 1",
                        Author = "Author 1",
                        Genre = MovieGenre.Action,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[0].Id,
                    },
                    new Movie
                    {
                        Title = "Movie 2",
                        Description = "Description 2",
                        Author = "Author 2",
                        Genre = MovieGenre.Comedy,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[0].Id,
                    },
                    new Movie
                    {
                        Title = "Movie 3",
                        Description = "Description 3",
                        Author = "Author 3",
                        Genre = MovieGenre.Action,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[0].Id,
                    },
                    new Movie
                    {
                        Title = "Movie 4",
                        Description = "Description 4",
                        Author = "Author 4",
                        Genre = MovieGenre.Drama,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[1].Id,
                    },
                    new Movie
                    {
                        Title = "Movie 5",
                        Description = "Description 5",
                        Author = "Author 5",
                        Genre = MovieGenre.Comedy,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[1].Id,
                    },
                    new Movie
                    {
                        Title = "Movie 6",
                        Description = "Description 6",
                        Author = "Author 6",
                        Genre = MovieGenre.ScienceFiction,
                        DateOfRelease = DateTime.Now,
                        UserId = createdUsers[0].Id,
                    }

                };
                _dataContext.Movies.AddRange(movies);
                await _dataContext.SaveChangesAsync();

            }
        }
    }
}
