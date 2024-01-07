using AutoMapper;
using FerozeHub.Services.CouponAPI.Data;
using FerozeHub.Services.CouponAPI.Models;
using FerozeHub.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FerozeHub.Services.CouponAPI.Controllers;

[Route("api/couponAPI")]
[ApiController]
public class CouponController(ApplicationDbContext dbContext,IMapper mapper) : BaseController 
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<Coupon> objList = dbContext.Coupons.ToList();
            var result = mapper.Map<IEnumerable<CouponDto>>(objList);
            return CreateResponse(result,true,"Coupon retrived successfully");
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
    [HttpGet]
    [Route("{id:int}")]
    public IActionResult Get(int id)
    {
        try
        {
            var obj=dbContext.Coupons.First(u=>u.CouponId == id);
            var response = mapper.Map<CouponDto>(obj);
            return CreateResponse(response,true,"Success");
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
    [HttpGet]
    [Route("GetByCode{code}")]
    public IActionResult GetByCode(string code)
    {
        try
        {
            var obj=dbContext.Coupons.FirstOrDefault(u=>u.CouponCode.ToLower() == code.ToLower());
            if (obj != null)
            {
                var response = mapper.Map<CouponDto>(obj);
                return CreateResponse(response,true,"Success");
            }
            else
            {
                return CreateResponse(null, false, "Error");
            }
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
    [HttpPost]
    
    public IActionResult Create(CouponDto obj)
    {
        try
        {
            if (obj != null)
            {
                var coupon = mapper.Map<Coupon>(obj);
                dbContext.Coupons.Add(coupon);
                dbContext.SaveChanges();
                var response =mapper.Map<CouponDto>(obj);
                return CreateResponse(response,true,"Success");
            }
            else
            {
                return CreateResponse(null, false, "Error");
            }
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
    [HttpPut]
    
    public IActionResult Update(CouponDto obj)
    {
        try
        {
            if (obj != null)
            {
                var coupon = mapper.Map<Coupon>(obj);
                dbContext.Coupons.Update(coupon);
                dbContext.SaveChanges();
                var response =mapper.Map<CouponDto>(obj);
                return CreateResponse(response,true,"Success");
            }
            else
            {
                return CreateResponse(null, false, "Error");
            }
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
    
    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var coupon = dbContext.Coupons.FirstOrDefault(c => c.CouponId == id);
                dbContext.Coupons.Remove(coupon);
                dbContext.SaveChanges();
                return CreateResponse(null, true, "Coupon deleted successfully");
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
}