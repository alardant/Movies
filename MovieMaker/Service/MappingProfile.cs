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
        CreateMap<Movie, MovieDto>()
           .ForMember(dest => dest.DateOfRelease, opt => opt.MapFrom(src => src.DateOfRelease));

        CreateMap<MovieDto, Movie>()
            .ForMember(dest => dest.DateOfRelease, opt => opt.MapFrom(src => src.DateOfRelease));

        CreateMap<User, UserToDisplayDto>();
        CreateMap<UserDto, User>();
        CreateMap<UserLoginDto, User>();
    }
}