namespace Application.Exceptions
{
    /// <summary>
    /// Exceção customizada para erros de validação na camada Application.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}
