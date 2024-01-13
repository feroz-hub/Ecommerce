using AutoMapper;
using FerozeHub.Services.ProductAPI.Data;
using FerozeHub.Services.ProductAPI.Models;
using FerozeHub.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FerozeHub.Services.ProductAPI.Controllers;
[Route("api/productAPI")]
[ApiController]

public class ProductAPIController(ApplicationDbContext dbContext,IMapper mapper) : BaseController
{
   [HttpGet]
    public IActionResult Get()
    {
        try
        {
            IEnumerable<Product> objList = dbContext.Products.ToList();
            var result = mapper.Map<IEnumerable<ProductDto>>(objList);
            return CreateResponse(result,true,"Products retrived successfully");
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
            var obj=dbContext.Products.First(u=>u.ProductId == id);
            var response = mapper.Map<ProductDto>(obj);
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
            var obj=dbContext.Products.FirstOrDefault(u=>u.Name.ToLower() == code.ToLower());
            if (obj != null)
            {
                var response = mapper.Map<ProductDto>(obj);
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
    [Authorize(Roles = "ADMIN")]
    public IActionResult Create(ProductDto obj)
    {
        try
        {
            if (obj != null)
            {
                var coupon = mapper.Map<Product>(obj);
                dbContext.Products.Add(coupon);
                dbContext.SaveChanges();
                var response =mapper.Map<ProductDto>(obj);
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
    [Authorize(Roles = "ADMIN")]

    public IActionResult Update(ProductDto obj)
    {
        try
        {
            if (obj != null)
            {
                var product = mapper.Map<Product>(obj);
                dbContext.Products.Update(product);
                dbContext.SaveChanges();
                var response =mapper.Map<ProductDto>(obj);
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
    [Authorize(Roles = "ADMIN")]
    public IActionResult Delete(int id)
    {
        try
        {
            var product = dbContext.Products.FirstOrDefault(c => c.ProductId == id);
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
                return CreateResponse(null, true, "Product deleted successfully");
        }
        catch (Exception exception)
        {
            return CreateResponse(null, false, exception.Message);
        }
    }
}