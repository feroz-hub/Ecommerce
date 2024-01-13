using FerozeHub.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FerozeHub.Services.ProductAPI.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult CreateResponse(object result, bool success, string message)
    {
        if (success)
        {
            return Ok(new ResponseDto
            {
                Result = result,
                IsSuccess = success,
                Message = message
            });
        }
        else
        {
            return BadRequest(new ResponseDto
            {
                Result = result,
                IsSuccess = success,
                Message = message
            });
        }
    }
  
}