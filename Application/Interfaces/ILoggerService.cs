using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoggerService
    {
        Task LogTraceAsync(Trace trace);

        Task LogRequestAsync(RequestLog logEntry);
        Task UpdateRequestLogAsync(RequestLog logEntry);
    }
}
