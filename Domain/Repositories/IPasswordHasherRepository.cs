namespace Domain.Repositories
{
    public interface IPasswordHasherRepository
    {
        string HashPassword(string plainPassword);
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
