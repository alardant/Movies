using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieMaker.Data;
using MovieMaker.Models;
using MovieMaker.Services;

namespace MovieMaker.Repository
{
    /// <summary>
    /// Repository for managing sers.
    /// </summary>
    public class UserRepository
    {
        private readonly DataContext _context;
        private UserManager<User> _userManager;
        public UserRepository(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a user by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The movie with the specified identifier.</returns>
        public async Task<User> GetUserByIdAsync(string Id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        }

        /// <summary>
        /// Gets all users .
        /// </summary>
        /// <returns>List of users.</returns>
        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The updated user.</param>
        /// /// <param name="passwordUpdated">The updated password.</param>
        /// <returns>True if the user is updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateUserAsync(User user, string passwordUpdated)
        {
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded && !string.IsNullOrEmpty(passwordUpdated))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, passwordUpdated);

                if (!resetPasswordResult.Succeeded)
                {
                    return false;
                }
            }

            return Save();
        }

        /// <summary>
        /// Checks if a user with the specified identifier exists.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public async Task<bool> DoesUserExists(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns>True if changes are saved successfully; otherwise, false.</returns>
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
