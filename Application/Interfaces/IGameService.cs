using Application.DTO.Game;

namespace Application.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<GameResponse>> GetAllGamesAsync();
        GameSearchResponse SearchGames(GameSearchRequest request, int page = 0, int pageSize = 10);
        List<GameDocument> GetTopRatedGames(int top = 10);
        GameResponse GetGameById(int id);
        GameResponse AddGame(GameRequest game);
        GameResponse UpdateGame(GameRequest game);
        bool DeleteGame(int id);
    }
}
