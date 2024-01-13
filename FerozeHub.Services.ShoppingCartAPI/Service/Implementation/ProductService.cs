using FerozeHub.Services.ShoppingAPI.Models.Dto;
using FerozeHub.Services.ShoppingAPI.Service.Interface;
using Newtonsoft.Json;

namespace FerozeHub.Services.ShoppingAPI.Service.Implementation;

public class ProductService(IHttpClientFactory httpClientFactory):IProductService
{
    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var client = httpClientFactory.CreateClient("Product");
        var products = await client.GetAsync($"/api/productAPI");
        var apiContent = await products.Content.ReadAsStringAsync();
        var response=JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        if (response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(response.Result));
        }

        return new List<ProductDto>();
    }
}