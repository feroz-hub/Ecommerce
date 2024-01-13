using AutoMapper;
using FerozeHub.Services.ShoppingAPI.Models;
using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<CartHeader,CartHeaderDto>().ReverseMap();
            config.CreateMap<CartDetails,CartDetailsDto>().ReverseMap();
        });
        return mapper;
    }
}