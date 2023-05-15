using AutoMapper;
using CarManager.Models;
using CarManager.Models.Dto;

namespace CarManager.Services.MapperProfiles;

public class CarMapperProfile : Profile
{
    public CarMapperProfile()
    {
        CreateMap<CarDto, Car>();
    }
}