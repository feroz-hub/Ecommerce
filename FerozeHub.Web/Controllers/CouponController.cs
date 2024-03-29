using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FerozeHub.Web.Controllers;

public class CouponController(ICouponService couponService) : Controller
{
    // GET
    public async Task<IActionResult> IndexCoupon()
    {
        List<CouponDto>? list = new();
        ResponseDto response =await couponService.GetAllCouponAsync();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }
 
    public async Task<IActionResult> CreateCoupon()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult>CreateCoupon(CouponDto couponDto)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response=await couponService.CreateCouponAsync(couponDto);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Created Successfully";
                return RedirectToAction(nameof(IndexCoupon));
            }
            else
            {
                TempData["error"]=response?.Message;
            }
        }
		return View(couponDto);
 
	}
 
    public async Task<IActionResult> DeleteCoupon(int couponId)
    {
        ResponseDto? response= await couponService.GetCouponByIdAsync(couponId);
        if(response != null && response.IsSuccess)
        {
            CouponDto? couponDto = JsonConvert.DeserializeObject<CouponDto>(response.Result.ToString());
            return View(couponDto);
        }
        else
        {
            TempData["error"] = response?.Message;
            
        }
        return NotFound();
    }
 
    [HttpPost]
    public async Task<IActionResult> DeleteCoupon(CouponDto couponDto)
    {
        ResponseDto? response = await couponService.DeleteCouponAsync(couponDto.CouponId);
        if(response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon Deleted Successfully";
            return RedirectToAction(nameof(IndexCoupon));
        }
        else
        {
            TempData["error"]=response?.Message;
        }
        return View(couponDto);
    }
}