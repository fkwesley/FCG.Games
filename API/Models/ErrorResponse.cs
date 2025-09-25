namespace API.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string? Detail { get; set; }
        public Guid? LogId { get; set; } 
    }
}
