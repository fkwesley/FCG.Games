using Application.DTO.Game;
using Application.Mappings;
using Domain.Entities;
using FluentAssertions;

namespace FCG.Tests.UnitTests.FCG.Tests.Application.Mappings
{
    public class GameMappingExtensionsTests
    {
        [Fact]
        public void ToEntity_ShouldMapCorrectly()
        {
            var request = new GameRequest
            {
                GameId = 1,
                Name = "Halo",
                Description = "Sci-fi shooter",
                Genre = "FPS",
                Rating = 5,
                ReleaseDate = new DateOnly(2020, 1, 1)
            };

            var result = request.ToEntity();

            result.GameId.Should().Be(1);
            result.Name.Should().Be("Halo");
            result.Genre.Should().Be("FPS");
        }

        [Fact]
        public void ToResponse_ShouldMapCorrectly()
        {
            var entity = new Game
            {
                GameId = 1,
                Name = "Halo",
                Description = "Sci-fi shooter",
                Genre = "FPS",
                Rating = 5,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                ReleaseDate = new DateOnly(2020, 1, 1)
            };

            var result = entity.ToResponse();

            result.Name.Should().Be("Halo");
            result.CreatedAt.Kind.Should().Be(DateTimeKind.Local);
        }
    }
}
