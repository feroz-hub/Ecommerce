using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI.Service.Interface;

public interface ICouponService
{
    Task<CouponDto> GetCoupon(string couponCode);
}