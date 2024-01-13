using FerozeHub.Web.Models.Dto;

namespace FerozeHub.Web.Service.Interfaces;

public interface IProductService
{
    public Task<ResponseDto?> GetProductAsync(string name);
    public Task<ResponseDto?> GetProductByIdAsync(int id);
    public Task<ResponseDto?> GetAllProductAsync();
    public Task<ResponseDto?> CreateProductAsync(ProductDto product);
    public Task<ResponseDto?> UpdateProductAsync(ProductDto product);
    public Task<ResponseDto?> DeleteProductAsync(int id);
}