using Application.DTO.Game;
using Domain.Entities;
using Application.Helpers;

namespace Application.Mappings
{
    public static class GameMappingExtensions
    {
        /// <summary>   
        /// Maps a GameRequest to a Game entity.
        public static Game ToEntity(this GameRequest request)
        {
            return new Game
            {
                GameId = request.GameId,
                Name = request.Name,
                Description = request.Description,
                Genre = request.Genre,
                Price = request.Price,
                ReleaseDate = request.ReleaseDate,
                Rating = request.Rating
            };
        }

        /// <summary>
        /// maps a Game entity to a GameResponse.
        public static GameResponse ToResponse(this Game entity)
        {
            return new GameResponse
            {
                GameId = entity.GameId,
                Name = entity.Name,
                Description = entity.Description,
                Genre = entity.Genre,
                Price = entity.Price,
                ReleaseDate = entity.ReleaseDate,
                CreatedAt = DateTimeHelper.ConvertUtcToTimeZone(entity.CreatedAt, "E. South America Standard Time"),
                UpdatedAt = entity.UpdatedAt.HasValue ? 
                                DateTimeHelper.ConvertUtcToTimeZone(entity.UpdatedAt.Value, "E. South America Standard Time") : (DateTime?)null,
                Rating = entity.Rating
            };
        }

        /// <summary>
        /// Maps a Game to a GameDocument.
        public static GameDocument ToDocument(this Game entity)
        {
            return new GameDocument
            {
                Id = entity.GameId.ToString(),
                Name = entity.Name,
                Description = entity.Description,
                Genre = entity.Genre,
                Price = entity.Price,
                ReleaseDate = entity.ReleaseDate,
                Rating = entity.Rating,
            };
        }
    }
}
