using FerozeHub.Web.Models.Dto;

namespace FerozeHub.Web.Service.Interfaces;

public interface IBaseService
{
    public Task<ResponseDto?> SendAsync(RequestDto request);
}
