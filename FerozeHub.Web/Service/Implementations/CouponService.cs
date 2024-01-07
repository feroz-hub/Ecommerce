using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

namespace FerozeHub.Web.Service.Implementations;

public class CouponService(IBaseService baseService):ICouponService
{
    public async Task<ResponseDto?> GetCouponAsync(string code)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType =ApiType.GET,
            Url = SD.CouponAPI+"/api/couponAPI/GetByCode/"+code
        });

    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.GET,
            Url = SD.CouponAPI + "/api/couponAPI/"+id
        });
    }

    public async Task<ResponseDto?> GetAllCouponAsync()
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.GET,
            Url = SD.CouponAPI + "/api/couponAPI"
        });
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto coupon)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CouponAPI + "/api/couponAPI",
            ApiType = ApiType.POST,
            Data = coupon
        });
    }

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto coupon)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CouponAPI + "/api/couponAPI/",
            ApiType = ApiType.PUT,
            Data = coupon
        });
    }

    public async Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CouponAPI+ "/api/couponAPI/" + id,
            ApiType = ApiType.DELETE
        });
    }
}