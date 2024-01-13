using AutoMapper;
using FerozeHub.Services.ProductAPI.Models;
using FerozeHub.Services.ProductAPI.Models.Dto;

namespace FerozeHub.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<Product,ProductDto>().ReverseMap();
        });
        return mapper;
    }
}