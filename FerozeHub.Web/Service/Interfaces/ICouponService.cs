using FerozeHub.Web.Models.Dto;

namespace FerozeHub.Web.Service.Interfaces;

public interface ICouponService
{
    public Task<ResponseDto?> GetCouponAsync(string coupon);
    public Task<ResponseDto?> GetCouponByIdAsync(int id);
    public Task<ResponseDto?> GetAllCouponAsync();
    public Task<ResponseDto?> CreateCouponAsync(CouponDto coupon);
    public Task<ResponseDto?> UpdateCouponAsync(CouponDto coupon);
    public Task<ResponseDto?> DeleteCouponAsync(int id);
}