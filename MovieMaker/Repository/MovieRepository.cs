﻿using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Data;
using Movies.Models;
using Movies.Controllers;

namespace Movies.Repository
{
    /// <summary>
    /// Repository for managing movies.
    /// </summary>
    public class MovieRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(DataContext context, ILogger<MovieRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all movies asynchronously.
        /// </summary>
        /// <returns>The list of movies.</returns>
        public async Task<ICollection<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        /// <summary>
        /// Gets a movie by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The movie with the specified identifier.</returns>
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Gets movies associated with a user asynchronously.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The list of movies associated with the user.</returns>
        public async Task<ICollection<Movie>> GetMoviesByUserAsync(string id)
        {
            return await _context.Movies.Where(i => i.UserId == id).ToListAsync();
        }

        /// <summary>
        /// Creates a new movie asynchronously.
        /// </summary>
        /// <param name="movie">The movie to be created.</param>
        /// <returns>True if the movie is created successfully; otherwise, false.</returns>
        public async Task<bool> CreateMovieAsync(Movie movie)
        {

            _context.Movies.Add(movie);
            return await Save();
        }

        /// <summary>
        /// Updates an existing movie asynchronously.
        /// </summary>
        /// <param name="movie">The updated movie.</param>
        /// <returns>True if the movie is updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return await Save();
        }

        /// <summary>
        /// Deletes a movie by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>True if the movie is deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movieToDelete = await _context.Movies.FindAsync(id);

            if (movieToDelete == null)
            {
                return false;
            }


            _context.Remove(movieToDelete);
            _context.SaveChanges();
            return await Save();
        }

        /// <summary>
        /// Checks if a movie with the specified identifier exists.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>True if the movie exists; otherwise, false.</returns>
        public async Task<bool> DoesMovieExists(int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(i => i.Id == id);
            if (movie == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns>True if changes are saved successfully; otherwise, false.</returns>
        public async Task<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            } catch (DbUpdateException ex)
            {
                _logger.LogError($"Error saving changes to the database: {ex.Message}");
                return false;
            }

        }


    }
}
