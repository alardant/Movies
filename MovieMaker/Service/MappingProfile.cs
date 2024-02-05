using AutoMapper;
using Movies.DTO;
using Movies.Models;

public class MappingProfile : Profile
{
    /// <summary>
    /// Service for mapping.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Movie, MovieDto>();
        CreateMap<User, UserToDisplayDto>();
        CreateMap<UserDto, User>();
        CreateMap<UserLoginDto, User>();
    }
}