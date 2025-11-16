using EV.DataConsumerService.API.Models.DTOs;

namespace EV.DataConsumerService.API.Service
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> Register(UserRegisterDto userRegisterDto);
        Task<AuthResponseDto> Login(UserLoginDto userLoginDto);
    }
}
