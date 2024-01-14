using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

namespace FerozeHub.Web.Service.Implementations;

public class CartService(IBaseService baseService):ICartService
{
    public async Task<ResponseDto?> GetCartByUserIdAsync(string UserId)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType =ApiType.GET,
            Url = SD.CartAPI+"/api/cartAPI/GetCart/"+UserId
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CartAPI + "/api/cartAPI/CartUpsert",
            ApiType = ApiType.POST,
            Data = cartDto
        });
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartdetailsId)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CartAPI + "/api/cartAPI/RemoveCart",
            ApiType = ApiType.POST,
            Data = cartdetailsId
        });
    }

    public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CartAPI + "/api/cartAPI/ApplyCoupon",
            ApiType = ApiType.POST,
            Data = cartDto
        });
    }

    public async Task<ResponseDto?> EmailCart(CartDto cartDto)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.CartAPI + "/api/cartAPI/EmailCartRequest",
            ApiType = ApiType.POST,
            Data = cartDto
        });
    }
}