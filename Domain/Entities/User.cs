using Domain.Exceptions;
using Domain.Repositories;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class User
    {
        [DebuggerDisplay("UserId: {UserId}, Name: {Name}, IsActive: {IsActive}")]
        public required string UserId { get; set; }
        public required string Name { get; set; }

        private string _email { get; set; }
        public string PasswordHash { get; private set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsAdmin { get; set; } = false;
        public ICollection<RequestLog> RequestLogs { get; set; } = new List<RequestLog>();

        // Propriedade pública com validação de e-mail no `set`.
        public string Email
        {
            get => _email;
            set
            {
                if (!EmailRegex.IsMatch(value)) // Se o valor não corresponde ao padrão de e-mail...
                    throw new BusinessException("Invalid email format."); // ...lança exceção.
                _email = value; // Se válido, atribui ao campo privado.
            }
        }

        public void SetPassword(string plainPassword, IPasswordHasherRepository passwordHasher)
        {
            if (!StrongPasswordRegex.IsMatch(plainPassword))
                throw new BusinessException("Password must be at least 8 characters and include letters, numbers and special characters.");

            PasswordHash = passwordHasher.HashPassword(plainPassword);
        }

        #region BusinessRules
        // Expressão regular para validar formato básico de e-mail.
        // Garante que tenha algo antes e depois de @ e depois um ponto.
        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        // Expressão regular para validar senha forte:
        // - Pelo menos uma letra
        // - Pelo menos um número
        // - Pelo menos um caractere especial
        // - Mínimo de 8 caracteres
        private static readonly Regex StrongPasswordRegex =
            new(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", RegexOptions.Compiled);
        #endregion


    }
}
