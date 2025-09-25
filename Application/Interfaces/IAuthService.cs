using Application.DTO.Auth;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        LoginResponse GenerateToken(User user);
    }
}
