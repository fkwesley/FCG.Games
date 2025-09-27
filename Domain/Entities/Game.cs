
namespace Domain.Entities
{
    public class Game
    {
        public int GameId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Genre { get; set; }
        public required decimal Price { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int? Rating { get; set; }
    }
}
