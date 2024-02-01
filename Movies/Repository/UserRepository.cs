using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieMaker.Data;
using MovieMaker.Models;
using MovieMaker.Services;

namespace MovieMaker.Repository
{
    /// <summary>
    /// Repository for managing users.
    /// </summary>
    public class UserRepository
    {
        private readonly DataContext _context;
        private UserManager<User> _userManager;
        private AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DataContext"/> used for database operations.</param>
        /// <param name="userManager">The <see cref="UserManager{TUser}"/> used for user management.</param>
        /// <param name="authService">The <see cref="AuthService"/> providing authentication-related functionality.</param>
        public UserRepository(DataContext context, UserManager<User> userManager, AuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
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
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// /// /// <param name="password">The user password.</param>
        /// <returns>True if the user is created successfully; otherwise, false.</returns>
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var isUserAdmin = user.IsUserAdmin;

            if (isUserAdmin)
            {
                await _authService.AssignRolesAsync(user, isUserAdmin);

            }

            await _userManager.CreateAsync(user, password);
            return Save();
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
        /// Delete an existing user asynchronously.
        /// </summary>
        /// <param name="id">The Id of the existing user.</param>
        /// <returns>True if the user is deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            await _userManager.DeleteAsync(user);
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
