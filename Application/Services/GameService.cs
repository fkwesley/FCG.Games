using Application.DTO.Game;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IElasticService _elasticService;

        public GameService(
                IGameRepository gameRepository, 
                ILoggerService loggerService,
                IHttpContextAccessor httpContext,
                IServiceScopeFactory scopeFactory,
                IElasticService elasticService)
        {
            _gameRepository = gameRepository 
                ?? throw new ArgumentNullException(nameof(gameRepository));
            _httpContext = httpContext;
            _scopeFactory = scopeFactory;
            _elasticService = elasticService;
        }

        public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
        {
            var games = _gameRepository.GetAllGames();

            using var scope = _scopeFactory.CreateScope();
            var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();

            await loggerService.LogTraceAsync(new Trace
            {
                LogId = _httpContext.HttpContext?.Items["RequestId"] as Guid?,
                Timestamp = DateTime.UtcNow,
                Level = LogLevel.Info,
                Message = "Retrieved all games",
                StackTrace = null
            });

            return games.Select(game => game.ToResponse()).ToList();
        }

        public GameResponse GetGameById(int id)
        {
            var gameFound = _gameRepository.GetGameById(id);

            return gameFound.ToResponse();
        }

        public GameSearchResponse SearchGames(GameSearchRequest request, int page = 0, int pageSize = 10)
        {
            return _elasticService.SearchGamesAsync(request, page, pageSize).Result;
        }

        public List<GameDocument> GetTopRatedGames(int top = 10)
        {
            return _elasticService.GetTopRatedGamesAsync(top).Result.ToList();
        }

        public GameResponse AddGame(GameRequest game)
        {
            if (_gameRepository.GetAllGames().Any(g => g.Name == game.Name))
                throw new ValidationException(string.Format("Game {0} already exists.",game.Name));

            var gameEntity = game.ToEntity();
            var gameAdded = _gameRepository.AddGame(gameEntity);

            // Indexing the new game in Elasticsearch
            var gameDocument = gameAdded.ToDocument();
            _elasticService.IndexAsync(gameDocument, gameDocument.Id).Wait();

            return gameAdded.ToResponse();
        }

        public GameResponse UpdateGame(GameRequest game)
        {
            if (_gameRepository.GetAllGames().Any(g => g.Name == game.Name && g.GameId != game.GameId))
                throw new ValidationException(string.Format("Game {0} already exists.", game.Name));

            var gameEntity = game.ToEntity();
            var gameUpdated = _gameRepository.UpdateGame(gameEntity);

            return gameUpdated.ToResponse();
        }

        public bool DeleteGame(int id)
        {
            return _gameRepository.DeleteGame(id);
        }

    }
}
