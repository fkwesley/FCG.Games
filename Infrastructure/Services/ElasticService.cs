using Application.DTO.Game;
using Application.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services
{
    public class ElasticService : IElasticService
    {
        private readonly ElasticsearchClient _client;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public ElasticService(IConfiguration configuration, IHttpContextAccessor httpContext, IServiceScopeFactory scopeFactory)
        {
            var elasticUrl = configuration["ElasticSearch:ServerUrl"];
            var apiKey = configuration["ElasticSearch:ApiKey"];
            var defaultIndex = configuration["ElasticSearch:DefaultIndex"] ?? "games";

            var clientSettings = new ElasticsearchClientSettings(new Uri(elasticUrl))
                .Authentication(new Elastic.Transport.ApiKey(apiKey))
                .DefaultIndex(defaultIndex);

            _client = new ElasticsearchClient(clientSettings);
            _httpContext = httpContext;
            _scopeFactory = scopeFactory;
        }

        public async Task IndexAsync<T>(T document, string? id = null)
            where T : class
        {
            var response = await _client.IndexAsync(document, idx => idx
                .Id(id)
                .Refresh(Refresh.True));

            if (!response.IsValidResponse)
            {
                if (response.TryGetOriginalException(out var exception))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();

                    await loggerService.LogTraceAsync(new Domain.Entities.Trace
                    {
                        LogId = _httpContext.HttpContext?.Items["RequestId"] as Guid?,
                        Timestamp = DateTime.UtcNow,
                        Message = "Erro ao indexar documento",
                        StackTrace = exception.Message,
                        Level = Domain.Enums.LogLevel.Error
                    });

                    Console.WriteLine($"Erro ao indexar documento: {exception.Message}");
                }
                else
                    Console.WriteLine("Erro ao indexar documento: Erro desconhecido.");

                return;
            }

            Console.WriteLine($"Documento indexado com sucesso. Id: {response.Id}, Resultado: {response.Result}");

            var getResponse = await _client.GetAsync<T>(response.Id);
            if (getResponse.Found)
                Console.WriteLine("Documento confirmado no índice!");
            else
                Console.WriteLine("Documento não encontrado após indexação!");
        }

        public async Task<GameSearchResponse> SearchGamesAsync(GameSearchRequest request, int page = 0, int pageSize = 10)
        {
            var query = new List<Query>();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query.Add(new MatchQuery
                {
                    Field = "name",
                    Query = request.Name,
                    Fuzziness = "AUTO" // Enables fuzzy matching for approximate matches
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Genre))
            {
                query.Add(new MatchQuery
                {
                    Field = "genre.keyword", // Updated to use the 'keyword' field for aggregations
                    Query = request.Genre,
                    Fuzziness = "AUTO" // Enables fuzzy matching for approximate matches
                });
            }

            if (request.ReleaseDateFrom.HasValue || request.ReleaseDateTo.HasValue)
            {
                query.Add(new DateRangeQuery
                {
                    Field = "releaseDate",
                    Gte = request.ReleaseDateFrom,
                    Lte = request.ReleaseDateTo
                });
            }

            if (request.PriceFrom.HasValue || request.PriceTo.HasValue)
            {
                query.Add(new NumberRangeQuery
                {
                    Field = "price",
                    Gte = (Number?)request.PriceFrom,
                    Lte = (Number?)request.PriceTo
                });
            }

            var searchDescriptor = new SearchRequest<GameDocument>
            {
                Query = new BoolQuery { Must = query },
                From = page * pageSize,
                Size = pageSize
            };

            if (request.IncludeAggregations)
            {
                searchDescriptor.Aggregations = new Dictionary<string, Aggregation>
                {
                    {
                        "genre_counts", new Aggregation
                        {
                            Terms = new TermsAggregation
                            {
                                Field = "genre.keyword" // Updated to use the 'keyword' field for aggregations
                            }
                        }
                    }
                };
            }

            var response = await _client.SearchAsync<GameDocument>(searchDescriptor);

            if (!response.IsValidResponse)
                throw new Exception($"Search failed: {response.DebugInformation}");

            var result = new GameSearchResponse
            {
                Total = response.Total,
                Results = response.Documents.Select(doc => new GameDocument
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Description = doc.Description,
                    Genre = doc.Genre,
                    Price = doc.Price,
                    ReleaseDate = doc.ReleaseDate,
                    Rating = doc.Rating
                }).ToList()
            };

            return result;
        }

        public async Task<List<GameDocument>> GetTopRatedGamesAsync(int top = 10)
        {
            var searchResponse = await _client.SearchAsync<GameDocument>(s => s
                .Size(top)
                .Sort(ss => ss
                    .Field(f => f.Rating, SortOrder.Desc)
                )
            );

            if (!searchResponse.IsValidResponse)
                throw new Exception($"Search failed: {searchResponse.DebugInformation}");

            return searchResponse.Documents.Select(doc => new GameDocument
            {
                Id = doc.Id,
                Name = doc.Name,
                Description = doc.Description,
                Genre = doc.Genre,
                Price = doc.Price,
                ReleaseDate = doc.ReleaseDate,
                Rating = doc.Rating
            }).ToList();
        }
    }
}
