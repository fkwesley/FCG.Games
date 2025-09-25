using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly GamesDbContext _context;

        // Injeta o DbContext via construtor
        public LoggerRepository(GamesDbContext context)
        {
            _context = context;
        }

        public async Task LogTraceAsync(Trace log)
        {
            await _context.Traces.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogRequestAsync(RequestLog log)
        {
            await _context.RequestLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestLogAsync(RequestLog log)
        {
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }
}
