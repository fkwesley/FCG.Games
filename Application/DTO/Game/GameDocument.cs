using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Application.DTO.Game
{
    public class GameDocument   
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public DateOnly ReleaseDate { get; set; }

        public int? Rating { get; set; } = 0;     // Ex: classificação do jogo
    }

    public class GameSearchRequest
    {
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public DateTime? ReleaseDateFrom { get; set; }
        public DateTime? ReleaseDateTo { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool IncludeAggregations { get; set; } = false;
    }

    public class GameSearchResponse
    {
        public long Total { get; set; }
        public List<GameDocument> Results { get; set; } = new();
        public Dictionary<string, long>? GenreCounts { get; set; }
    }
}
