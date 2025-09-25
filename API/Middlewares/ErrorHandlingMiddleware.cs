using API.Models;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using System.Text.Json;
using LogLevel = Domain.Enums.LogLevel;

namespace API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;  // Logger padrão (console, arquivo, etc)
        private readonly IServiceScopeFactory _scopeFactory;        // Para resolver serviços Scoped no escopo correto

        public ErrorHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ErrorHandlingMiddleware> logger,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Executa o próximo middleware ou endpoint da pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Captura exceções e trata/loga
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Recupera o mesmo LogId gerado pelo middleware de request
            var logId = context.Items.ContainsKey("RequestId")
                ? (Guid)context.Items["RequestId"]
                : Guid.NewGuid(); // Garante que sempre haverá um LogId, mesmo se o middleware anterior falhou

            var trace = new Trace
            {
                LogId = logId,
                Timestamp = DateTime.UtcNow,
                Level = LogLevel.Error,
                Message = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty
            };

            try
            {
                // Cria escopo para resolver ILoggerService (Scoped) corretamente
                using var scope = _scopeFactory.CreateScope();
                var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();

                // Salva o log no banco via serviço de log
                await loggerService.LogTraceAsync(trace);
            }
            catch (Exception logEx)
            {
                // Se falhar o log no banco, registra no logger padrão para não perder informação
                _logger.LogError(logEx, "Falha ao salvar log no banco.");
            }

            // Loga no console ou arquivo para ajudar no desenvolvimento
            _logger.LogError(ex, "Exceção capturada pelo middleware.");

            // Configura o response da API para cliente
            context.Response.ContentType = "application/json";

            // Define status HTTP baseado no tipo da exceção
            context.Response.StatusCode = ex switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                BusinessException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            // Corpo da resposta JSON incluindo o GUID do log para rastreio
            var response = new ErrorResponse
            {
                Message = "An error occurred processing your request.",
                Detail = context.Response.StatusCode != StatusCodes.Status500InternalServerError ? ex.Message : "Contact our support and send the LogId returned.",
                LogId = context.Response.StatusCode == StatusCodes.Status500InternalServerError ? trace.LogId : null
            };

            var json = JsonSerializer.Serialize(response);

            // Envia resposta para o cliente
            await context.Response.WriteAsync(json);
        }
    }
}