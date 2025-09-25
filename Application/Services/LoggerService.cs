using Application.Interfaces;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http;
using Trace = Domain.Entities.Trace;

namespace Application.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILoggerRepository _loggerRepository;
        private readonly INewRelicLoggerRepository _newRelicLoggerRepository;
        private readonly bool _externalLoggerEnabled;

        public LoggerService(
            ILoggerRepository loggerRepository,
            INewRelicLoggerRepository newRelicLoggerRepository,
            IOptions<ExternalLoggerSettings> externalLogger)
        {
            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));
            _newRelicLoggerRepository = newRelicLoggerRepository ?? throw new ArgumentNullException(nameof(newRelicLoggerRepository));
            _externalLoggerEnabled = externalLogger?.Value?.Enabled ?? false;
        }

        public async Task LogTraceAsync(Trace trace)
        {
            if (trace == null || trace.LogId == null)
                throw new ArgumentNullException(nameof(trace), "Log entry or LogId cannot be null.");

            if (_externalLoggerEnabled)
                _newRelicLoggerRepository.SendLogAsync(trace);

            try
            {
                await _loggerRepository.LogTraceAsync(trace);
            }
            catch (Exception ex)
            {
                await _newRelicLoggerRepository.SendLogAsync(new Trace
                {
                    LogId = trace.LogId,
                    Timestamp = DateTime.UtcNow,
                    Level = LogLevel.Error,
                    Message = "Error logging trace async: " + ex.Message,
                    StackTrace = ex.StackTrace
                });

                Console.WriteLine("Erro LogTraceAsync Message - " + ex.Message);
                Console.WriteLine();
                Console.WriteLine("Erro LogTraceAsync StackTrace - " + ex.StackTrace);
                Console.WriteLine();
                throw;
            }
        }

        public async Task LogRequestAsync(RequestLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log), "Log cannot be null.");

            if (_externalLoggerEnabled && log.StatusCode != 0)
                _newRelicLoggerRepository.SendLogAsync(log);

            try
            {
                await _loggerRepository.LogRequestAsync(log);
            }
            catch (Exception ex)
            {
                await _newRelicLoggerRepository.SendLogAsync(new Trace
                {
                    LogId = log.LogId,
                    Timestamp = DateTime.UtcNow,
                    Level = LogLevel.Error,
                    Message = "Error logging request async: " + ex.Message,
                    StackTrace = ex.StackTrace
                });

                Console.WriteLine("Erro LogRequestAsync Message - " + ex.Message);
                Console.WriteLine();
                Console.WriteLine("Erro LogRequestAsync StackTrace - " + ex.StackTrace);
                Console.WriteLine();
                throw;
            }
        }

        public async Task UpdateRequestLogAsync(RequestLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log), "Log entry cannot be null.");

            if (_externalLoggerEnabled && log.StatusCode != 0)
                _newRelicLoggerRepository.SendLogAsync(log);

            try
            {
                await _loggerRepository.UpdateRequestLogAsync(log);
            }
            catch (Exception ex)
            {
                await _newRelicLoggerRepository.SendLogAsync(new Trace
                {
                    LogId = log.LogId,
                    Timestamp = DateTime.UtcNow,
                    Level = LogLevel.Error,
                    Message = "Error updating RequestLog Async: " + ex.Message,
                    StackTrace = ex.StackTrace
                });

                Console.WriteLine("Erro UpdateRequestLogAsync Message - " + ex.Message);
                Console.WriteLine();
                Console.WriteLine("Erro UpdateRequestLogAsync StackTrace - " + ex.StackTrace);
                Console.WriteLine();
                throw;
            }

        }
    }
}
