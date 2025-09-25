using Domain.Repositories;
using Domain.Entities;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesDbContext _context;

        public GameRepository(GamesDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Game> GetAllGames()
        {
           return _context.Games.ToList();
        }

        public Game GetGameById(int id)
        {
            return _context.Games.FirstOrDefault(g => g.GameId == id) 
                ?? throw new KeyNotFoundException($"Game with ID {id} not found.");
        }

        public Game AddGame(Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();
            return game;
        }

        public Game UpdateGame(Game game)
        {
            var existingGame = GetGameById(game.GameId);

            if (existingGame != null) {
                existingGame.Name = game.Name;
                existingGame.Description = game.Description;
                existingGame.Genre = game.Genre;
                existingGame.ReleaseDate = game.ReleaseDate;
                existingGame.UpdatedAt = DateTime.UtcNow; 
                existingGame.Rating = game.Rating;
                
                _context.Games.Update(existingGame);
                _context.SaveChanges();
            }
            else
                throw new KeyNotFoundException($"Game with ID {game.GameId} not found.");

            return existingGame;
        }

        public bool DeleteGame(int id)
        {
            var game = GetGameById(id);

            if (game != null)
            {
                _context.Games.Remove(game);
                _context.SaveChanges();
                return true;
            }
            else
                throw new KeyNotFoundException($"Game with ID {id} not found.");
        }

    }
}
