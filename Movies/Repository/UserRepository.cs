using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;

namespace Movies.Repository
{
    /// <summary>
    /// Repository for managing sers.
    /// </summary>
    public class UserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
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
    }
}
