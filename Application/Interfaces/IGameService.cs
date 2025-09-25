using Application.DTO.Game;

namespace Application.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<GameResponse>> GetAllGamesAsync();
        GameResponse GetGameById(int id);
        GameResponse AddGame(GameRequest game);
        GameResponse UpdateGame(GameRequest game);
        bool DeleteGame(int id);
    }
}
