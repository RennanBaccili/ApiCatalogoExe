using ApiCatalogo.Context;
using ApiCatalogo.DTOs.Mappins;
using ApiCatalogo.Exceptions;
using ApiCatalogo.Logging;
using ApiCatalogo.Models;
using ApiCatalogo.Repository;
using ApiCatalogo.Repository.IRepository;
using ApiCatalogo.Services;
using ApiCatalogo.Services.AuthenticationsServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Ativa os constroladores da API
builder.Services.AddControllers(options => options.Filters.Add(typeof(ApiExceptionFilter)))
                .AddJsonOptions(options =>
                { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; })
                .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the database context to the DI container.
string? mysqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// Register the database context as a service and configure it to use MySQL.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(mysqlConnection));


//Transient:  Cada vez que alguma classe solicitar esse serviço sera gerado uma nova instancia de objeto
builder.Services.AddTransient<IService, Service>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));

// --- Configurações de Segurança

// Recupera a chave secreta da configuração ou lança uma exceção se não estiver definida
var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!");
// Adiciona serviços de autenticação ao contêiner de injeção de dependência
builder.Services.AddAuthentication(options =>
{
    // Define o esquema de autenticação padrão para JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Define o esquema de desafio padrão para JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Habilita a persistência do token depois da autenticação
    options.SaveToken = true;
    // Define se a metadata HTTPS é necessária na autenticação. False para desenvolvimento local
    options.RequireHttpsMetadata = false;
    // Parâmetros de validação do token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // Habilita a validação do emissor do token
        ValidateIssuer = true,
        // Habilita a validação da audiência do token
        ValidateAudience = true,
        // Habilita a validação do tempo de vida do token
        ValidateLifetime = true,
        // Habilita a validação da chave de assinatura do emissor
        ValidateIssuerSigningKey = true,
        // Define o desvio de tempo permitido para a validação de tempo de vida do token
        ClockSkew = TimeSpan.Zero,
        // Recupera a audiência válida da configuração
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        // Recupera o emissor válido da configuração
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        // Define a chave de assinatura do emissor para a validação do token
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// --- Fim das Configurações de Segurança

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
