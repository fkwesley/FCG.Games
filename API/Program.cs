using Azure;
using API.Middlewares;
using API.Models;
using Application.Interfaces;
using Application.Services;
using Application.Settings;
using Domain.Repositories;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

#region initializing
var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var jwtKey = configuration["Jwt:Key"];
var jwtIssuer = configuration["Jwt:Issuer"];

var connectionString = configuration.GetConnectionString("FCGGamesDbConnection")
                            ?? throw new ArgumentNullException("Connection string 'FCGGamesDbConnection' not found.");

builder.Services.Configure<ExternalLoggerSettings>(builder.Configuration.GetSection("NewRelic"));
#endregion

#region services

#region authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,  // Verifica se o issuer (emissor) bate
            ValidateAudience = false, // Neste exemplo não validamos a audiência
            ValidateLifetime = true,  // Valida se o token ainda não expirou
            ValidateIssuerSigningKey = true, // Valida a assinatura do token
            ValidIssuer = jwtIssuer,  // Define o emissor esperado
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) // Chave usada para verificar assinatura
        };

        //Configurando mensagens amigáveis
        options.Events = new JwtBearerEvents
        {
            // Quando não autenticado (token inválido ou ausente)
            OnChallenge = context =>
            {
                context.HandleResponse(); // para evitar o retorno padrão

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = new ErrorResponse
                {
                    Message = "Invalid Token, not Authenticated."
                };

                var json = JsonSerializer.Serialize(result);
                return context.Response.WriteAsync(json);
            },

            // Quando autenticado, mas sem permissão (ex: não tem role)
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var result = new ErrorResponse
                {
                    Message = "Access Denied! You do not have permission to perform this operation."
                };

                var json = JsonSerializer.Serialize(result);
                return context.Response.WriteAsync(json);
            }
        };
    });
#endregion

#region swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FCG.Users.API", Version = "v1" });

    // Define o esquema de segurança JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer", // Nome do esquema (obrigatório)
        BearerFormat = "JWT", // Apenas informativo
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    // Define a aplicação do esquema de segurança aos endpoints protegidos
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>() // Sem escopos específicos
        }
    });
    // Configuração para habilitar summaries
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
#endregion

#region dependencyInjection
// Dependency Injection for Application Layer
builder.Services.AddSingleton<IAuthService>(new AuthService(jwtKey, jwtIssuer));
builder.Services.AddSingleton<IPasswordHasherRepository, PasswordHasherRepository>();

//services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();

//repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();
builder.Services.AddScoped<INewRelicLoggerRepository, NewRelicLoggerRepository>();

// Register the DbContext with dependency injection
builder.Services.AddDbContext<GamesDbContext>(options =>
{
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped);
#endregion

#region otherServices
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
#endregion

#region invalidModel
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .Select(x =>
            {
                var field = x.Key;
                var messages = x.Value!.Errors.Select(e => e.ErrorMessage);
                return $"{field}: {string.Join(" | ", messages)}";
            });

        var response = new ErrorResponse
        {
            Message = "One or more validation errors occurred.",
            Detail = string.Join(" || ", errors),
            LogId = null
        };

        return new BadRequestObjectResult(response);
    };
});
#endregion

#endregion services

var app = builder.Build();

#region middlewares
// middleware para uso do swagger
app.UseSwagger();
app.UseSwaggerUI();


// Middleware que valida autenticação JWT em cada requisição
app.UseAuthentication();

// Middleware que avalia autorizações (ex: [Authorize])
app.UseAuthorization();

// middleware para log de requisições
app.UseMiddleware<RequestLoggingMiddleware>();

// middleware para tratamento global de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

//middleware que força o redirecionamento automático de requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Endpoint routing
app.MapControllers();
#endregion

app.Run();
