using Application.DTO.Game;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IElasticService
    {
        Task IndexAsync<T>(T document, string? id = null) where T : class;

        Task<GameSearchResponse> SearchGamesAsync(GameSearchRequest request, int page = 0, int pageSize = 10);
        Task<List<GameDocument>> GetTopRatedGamesAsync(int top = 10);
    }
}