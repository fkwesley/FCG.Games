using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Application.DTO.Game
{
    public class GameRequest
    {
        [JsonIgnore]
        public int GameId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(30)]
        public required string Genre { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateOnly ReleaseDate { get; set; }

        public int? Rating { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
