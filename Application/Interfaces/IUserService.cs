using Application.DTO.User;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        UserResponse GetUserById(string userId);
        User? ValidateCredentials(string userId, string password);
    }
}