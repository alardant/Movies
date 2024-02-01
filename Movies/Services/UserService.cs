using MovieMaker.DTO;
using MovieMaker.Models;
using UsersMovieMaker.DTO;

namespace MovieMaker.Services
{
    /// <summary>
    /// Service for managing users.
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService() { }

        /// <summary>
        /// Converts a <see cref="User"/> object to a <see cref="UserToDisplayDto"/>.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The user to display DTO.</returns>
        public UserToDisplayDto ConvertUserToUserToDisplayDto(User user)
        {
            return new UserToDisplayDto
            {
                Username = user.UserName,
                Email = user.Email,
                IsUserAdmin = user.IsUserAdmin
            };
        }

        /// <summary>
        /// Converts a <see cref="UserDto"/> object to a <see cref="User"/>.
        /// </summary>
        /// <param name="userDto">The user Dto object.</param>
        /// <returns>The user to display DTO.</returns>
        public User ConvertUserDtoToUser(UserDto userDto)
        {
            return new User
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                IsUserAdmin = userDto.IsUserAdmin
            };
        }
    }
}
