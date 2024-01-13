using FerozeHub.Web.Models.Dto;

namespace FerozeHub.Web.Service.Interfaces;

public interface ICartService
{
    public Task<ResponseDto?>GetCartByUserIdAsync (string UserId);
    public Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
    public Task<ResponseDto?> RemoveFromCartAsync(int cartdetailsId);
    public Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto );
 
}