using FerozeHub.Services.ShoppingAPI.Models.Dto;

namespace FerozeHub.Services.ShoppingAPI.Service.Interface;

public interface IProductService
{
    public Task<IEnumerable<ProductDto>> GetProductsAsync();
}