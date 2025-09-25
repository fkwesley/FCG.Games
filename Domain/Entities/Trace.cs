using Domain.Enums;

namespace Domain.Entities
{
    public class Trace
    {
        public Guid? LogId { get; set; }
        public int TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }

        public RequestLog RequestLog { get; set; } // Propriedade de navegação para RequestLog associado
    }
}
