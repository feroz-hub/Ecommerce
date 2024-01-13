using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FerozeHub.Web.Controllers;

public class ProductController(IProductService productService) : Controller
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
 
    public async Task<IActionResult> Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult>Create(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response=await productService.CreateProductAsync(productDto);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"]=response?.Message;
            }
        }
		return View(productDto);
 
	}

    public async Task<IActionResult> Edit(int productId)
    {
        ResponseDto? response= await productService.GetProductByIdAsync(productId);
        if(response != null && response.IsSuccess)
        {
            ProductDto? productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
            return View(productDto);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Edit(ProductDto product)
    {
        ResponseDto? responseDto = await productService.UpdateProductAsync(product);
        if (responseDto != null && responseDto.IsSuccess)
        {
            TempData["success"] = "Product Updated Successfully";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = responseDto?.Message;
        }
    
        return View(product);
    }
 
    public async Task<IActionResult> Delete(int productId)
    {
        ResponseDto? response= await productService.GetProductByIdAsync(productId);
        if(response != null && response.IsSuccess)
        {
            ProductDto? productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
            return View(productDto);
        }
        else
        {
            TempData["error"] = response?.Message;
            
        }
        return NotFound();
    }
 
    [HttpPost]
    public async Task<IActionResult> Delete(ProductDto productDto)
    {
        ResponseDto? response = await productService.DeleteProductAsync(productDto.ProductId);
        if(response != null && response.IsSuccess)
        {
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"]=response?.Message;
        }
        return View(productDto);
    }
    
    
}