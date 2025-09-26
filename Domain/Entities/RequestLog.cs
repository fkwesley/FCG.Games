using System.Collections.Generic;

namespace Domain.Entities
{
    public class RequestLog
    {
        public Guid LogId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } // UserId pode ser nulo se não houver autenticação
        public string HttpMethod { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public string? RequestBody { get; set; } = null;
        public string? ResponseBody { get; set; } = null;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public TimeSpan Duration { get; set; }

        public IEnumerable<Trace>? Traces { get; set; } = null; // Propriedade de navegação para traces associados
    }
}
