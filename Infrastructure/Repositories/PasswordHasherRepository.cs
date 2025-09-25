using Domain.Repositories;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Infrastructure.Repositories
{
    public class PasswordHasherRepository : IPasswordHasherRepository
    {
        public string HashPassword(string plainPassword)
        {
            // Gera um salt aleatório  
            byte[] salt = RandomNumberGenerator.GetBytes(16); // 16 bytes  

            // Deriva a chave usando PBKDF2 com SHA256  
            var hash = KeyDerivation.Pbkdf2(
                password: plainPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32); // 256 bits  

            // Retorna o salt e o hash concatenados, codificados em Base64
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string plainPassword, string hashedPasswordWithSalt)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty", nameof(plainPassword));
            if (string.IsNullOrWhiteSpace(hashedPasswordWithSalt))
                throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPasswordWithSalt));

            // O hashedPasswordWithSalt deve ter o formato "salt.hash"
            var parts = hashedPasswordWithSalt.Split('.', 2);
            if (parts.Length != 2)
                throw new FormatException("Invalid hashed password format.");

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            // Gera o hash da senha fornecida usando o salt extraído  
            var computedHash = KeyDerivation.Pbkdf2(
                password: plainPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32); // 256 bits  

            // Compara o hash gerado com o armazenado  
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
    }
}
