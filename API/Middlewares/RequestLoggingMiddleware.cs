using API.Models;
using Application.Interfaces;
using Domain.Entities;
using System.Diagnostics;
using System.Text.Json;

namespace API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ignorar requisições para o Swagger (UI e JSON)
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Inicia o timer para calcular a duração da requisição
            var stopwatch = Stopwatch.StartNew();

            // Cria um GUID único
            var requestId = Guid.NewGuid();

            // Compartilha o RequestId com outros middlewares via HttpContext.Items
            context.Items["RequestId"] = requestId;

            // Preparar para leitura do body
            context.Request.EnableBuffering();
            var requestBody = string.Empty;

            using var scope = _scopeFactory.CreateScope();
            var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();

            try
            {
                // Ler requestBody antes de salvar log inicial
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                // Cria log inicial só com dados da entrada
                var logEntry = new RequestLog
                {
                    LogId = requestId,
                    UserId = context.User?.FindFirst("user_id")?.Value, //context.User?.Identity?.Name,
                    HttpMethod = context.Request.Method,
                    Path = context.Request.Path,
                    RequestBody = !string.IsNullOrWhiteSpace(requestBody) ? requestBody : null,
                    StartDate = DateTime.UtcNow,
                };

                // Salva o log inicial para já ter LogId no banco
                await loggerService.LogRequestAsync(logEntry);

                // Trocar response body para capturar saída
                var originalBodyStream = context.Response.Body;
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                // Executa o próximo middleware/endpoint
                await _next(context);

                stopwatch.Stop();

                // Ler response body
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                memoryStream.Position = 0;

                // Copiar resposta para stream original
                await memoryStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                // Atualiza log com dados finais
                logEntry.StatusCode = context.Response.StatusCode;
                logEntry.ResponseBody = !string.IsNullOrWhiteSpace(responseBody) ? responseBody : null;
                logEntry.EndDate = DateTime.UtcNow;
                logEntry.Duration = stopwatch.Elapsed;

                // Atualiza log no banco
                await loggerService.UpdateRequestLogAsync(logEntry);
            }
            catch (Exception ex)
            { 
                // Configura o response da API para cliente
                context.Response.ContentType = "application/json";

                // Define status HTTP baseado no tipo da exceção
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new ErrorResponse
                {
                    Message = "An error occurred processing your request.",
                    Detail = ex.Message,
                    LogId = requestId
                };

                var json = JsonSerializer.Serialize(response);

                // Envia resposta para o cliente
                await context.Response.WriteAsync(json);
            }
        }
    }
}
