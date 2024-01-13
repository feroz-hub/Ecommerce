using System.IdentityModel.Tokens.Jwt;
using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FerozeHub.Web.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedonLoggedInUser());
    }

    private async Task<CartDto> LoadCartDtoBasedonLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto response=await cartService.GetCartByUserIdAsync(userId);
        if (response != null && response.IsSuccess)
        {
            CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            return cart;
        }

        return new CartDto();
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId=User.Claims.Where(u=>u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto response = await cartService.RemoveFromCartAsync(cartDetailsId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Cart Removed Successfully";
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }
    
    
    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        ResponseDto responseDto = await cartService.ApplyCouponAsync(cartDto);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Coupon Applied successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        cartDto.CartHeader.CouponCode="";
        ResponseDto responseDto = await cartService.ApplyCouponAsync(cartDto);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Coupon Applied successfully";
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }
}