using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Repositories
{
    public class NewRelicLoggerRepository : INewRelicLoggerRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _licenseKey;
        private readonly string _endpoint;

        public NewRelicLoggerRepository(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            
            _licenseKey = configuration["NewRelic:LicenseKey"]
                ?? throw new ArgumentNullException("NewRelic:LicenseKey not configured");

            _endpoint = configuration["NewRelic:Endpoint"]
                ?? throw new ArgumentNullException("NewRelic:LogEndpoint not configured");

            _httpClient.DefaultRequestHeaders.Add("X-License-Key", _licenseKey);
        }

        public async Task SendLogAsync(object logObject)
        {
            if (logObject == null)
                throw new ArgumentNullException(nameof(logObject));
            
            // valor default
            var level = "INFO";
            var message = "No message provided";

            // Verifica o tipo e extrai o nível apropriado
            switch (logObject)
            {
                case Trace trace:
                    level = trace.Level.ToString().ToUpper();
                    message = trace.Message;
                    break;

                case RequestLog requestLog:
                    level = requestLog.StatusCode >= 500 ? "ERROR"
                          : requestLog.StatusCode >= 400 ? "WARNING"
                          : "INFO";
                    message = string.Format("{0} {1} - {2}", requestLog.HttpMethod, requestLog.Path, requestLog.StatusCode);
                    break;
            }


            var payload = new[]
            {
                new
                {
                    message = message,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    attributes = new {
                        level,
                        log = logObject 
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_endpoint, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Falha ao enviar log para o New Relic: {ex.Message}");
            }
        }
    }
}
