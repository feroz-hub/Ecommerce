using FerozeHub.Web.Models.Dto;
using FerozeHub.Web.Service.Interfaces;
using FerozeHub.Web.Utility;

namespace FerozeHub.Web.Service.Implementations;

public class ProductService(IBaseService baseService):IProductService
{
    public async Task<ResponseDto?> GetProductAsync(string name)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = ApiType.GET,
            Url = SD.ProductAPI + "/api/productAPI/GetByCode/" + name
        });
    }

    public async Task<ResponseDto?> GetProductByIdAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.GET,
            Url = SD.ProductAPI + "/api/productAPI/"+id
        });
    }

    public async Task<ResponseDto?> GetAllProductAsync()
    {
        return await baseService.SendAsync(new RequestDto()
        {
            ApiType = ApiType.GET,
            Url = SD.ProductAPI + "/api/productAPI"
        });
    }

    public async Task<ResponseDto?> CreateProductAsync(ProductDto product)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.ProductAPI + "/api/productAPI",
            ApiType = ApiType.POST,
            Data = product
        });
    }

    public async Task<ResponseDto?> UpdateProductAsync(ProductDto product)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.ProductAPI + "/api/productAPI/",
            ApiType = ApiType.PUT,
            Data = product
        });
    }

    public async Task<ResponseDto?> DeleteProductAsync(int id)
    {
        return await baseService.SendAsync(new RequestDto()
        {
            Url = SD.ProductAPI+ "/api/productAPI/" + id,
            ApiType = ApiType.DELETE
        });
    }
}