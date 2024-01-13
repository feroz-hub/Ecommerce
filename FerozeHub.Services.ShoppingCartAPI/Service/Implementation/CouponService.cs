using FerozeHub.Services.ShoppingAPI.Models.Dto;
using FerozeHub.Services.ShoppingAPI.Service.Interface;
using Newtonsoft.Json;

namespace FerozeHub.Services.ShoppingAPI.Service.Implementation;

public class CouponService(IHttpClientFactory httpClientFactory):ICouponService
{
    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var client = httpClientFactory.CreateClient("Coupon");
        var coupon = await client.GetAsync($"/api/couponAPI/GetByCode/{couponCode}");
        var apiContent = await coupon.Content.ReadAsStringAsync();
        var response=JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (response!=null && response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
        }

        return new CouponDto();
    }
}