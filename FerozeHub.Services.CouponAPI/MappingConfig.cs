using AutoMapper;
using FerozeHub.Services.CouponAPI.Models;
using FerozeHub.Services.CouponAPI.Models.Dto;

namespace FerozeHub.Services.CouponAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<Coupon, CouponDto>().ReverseMap();
        });
        return mapper;
    }
    
}