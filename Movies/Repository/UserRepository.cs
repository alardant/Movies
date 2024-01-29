using Microsoft.EntityFrameworkCore;
using Movies.Data;
using Movies.Models;

namespace Movies.Repository
{
    public class UserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string Id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
