using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        User? GetUserById(string userId);
    }
}
