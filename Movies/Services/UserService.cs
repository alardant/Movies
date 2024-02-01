using MovieMaker.DTO;
using MovieMaker.Models;
using UsersMovieMaker.DTO;

namespace MovieMaker.Services
{
    public class UserService
    {
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
