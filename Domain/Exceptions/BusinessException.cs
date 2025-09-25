using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// <summary>
    /// Representa uma exceção de regra de negócio violada.
    /// Deve ser usada para erros previsíveis relacionados à lógica da aplicação.
    /// </summary>
    public class BusinessException : Exception
    {
        public string? ErrorCode { get; }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
