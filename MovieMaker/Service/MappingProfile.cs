﻿using AutoMapper;
using Movies.DTO;
using Movies.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Movie, MovieDto>()
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.ToString()))
            .ForMember(dest => dest.DateOfRelease, opt => opt.MapFrom(src => src.DateOfRelease.ToString("dd-MM-yyyy")));
    }
}