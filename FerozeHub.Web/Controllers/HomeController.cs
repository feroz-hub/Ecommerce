using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FerozeHub.Web.Models;
using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace FerozeHub.Web.Controllers;

public class HomeController(IProductService productService,ICartService cartService) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<ProductDto>? list = new();
        ResponseDto response =await productService.GetAllProductAsync();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }

    [Authorize]
    public async Task<IActionResult> Details(int productId)
    {
        ProductDto product = new();
        ResponseDto response =await productService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(product);
       
    }
    
    [Authorize]
    [HttpPost]
    
    public async Task<IActionResult> Details(ProductDto productDto)
    {
        CartDto cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto()
            {
                UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
            }
        };
        CartDetailsDto cartDetails = new CartDetailsDto()
        {
            Count = productDto.Count,
            ProductId = productDto.ProductId
        };

        List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
        cartDto.CartDetails=cartDetailsDtos;
        ProductDto product = new();
        ResponseDto response = await cartService.UpsertCartAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Item has been added to shopping cart successfully";

            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(productDto);
       
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}