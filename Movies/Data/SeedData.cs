using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MovieMaker.Models;
using Movies.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Movies.Data
{
    public class SeedData
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SeedData> _logger;

        public SeedData(
            UserManager<User> userManager,
            DataContext dataContext,
            RoleManager<IdentityRole> roleManager,
            ILogger<SeedData> logger)
        {
            _userManager = userManager;
            _dataContext = dataContext;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedDataAsync()
        {
            try
            {
                // Ensure role Admin exists
                await SeedRolesAsync();

                // Seed Users
                var createdUsers = await SeedUsersAsync();

                // Seed Movies
                await SeedMoviesAsync(createdUsers);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"An error occurred during seed data: {ex.Message}");
                // Handle the exception or rethrow it depending on your needs
            }
        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        private async Task<List<User>> SeedUsersAsync()
        {
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

            var createdUsers = new List<User>();

            foreach (var user in users)
            {
                var userCreationResult = await _userManager.CreateAsync(user, "Password123!");
                if (userCreationResult.Succeeded && user.UserName == "User1")
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                // Add the user to the list whether or not creation is successful
                createdUsers.Add(user);
            }

            await _dataContext.SaveChangesAsync(); // Save changes after seeding users
            return createdUsers;
        }

        private async Task SeedMoviesAsync(List<User> createdUsers)
        {
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
