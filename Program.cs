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


//Transient:  Cada vez que alguma classe solicitar esse servi�o sera gerado uma nova instancia de objeto
builder.Services.AddTransient<IService, Service>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));

// --- Configura��es de Seguran�a

// Recupera a chave secreta da configura��o ou lan�a uma exce��o se n�o estiver definida
var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!");
// Adiciona servi�os de autentica��o ao cont�iner de inje��o de depend�ncia
builder.Services.AddAuthentication(options =>
{
    // Define o esquema de autentica��o padr�o para JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Define o esquema de desafio padr�o para JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Habilita a persist�ncia do token depois da autentica��o
    options.SaveToken = true;
    // Define se a metadata HTTPS � necess�ria na autentica��o. False para desenvolvimento local
    options.RequireHttpsMetadata = false;
    // Par�metros de valida��o do token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        // Habilita a valida��o do emissor do token
        ValidateIssuer = true,
        // Habilita a valida��o da audi�ncia do token
        ValidateAudience = true,
        // Habilita a valida��o do tempo de vida do token
        ValidateLifetime = true,
        // Habilita a valida��o da chave de assinatura do emissor
        ValidateIssuerSigningKey = true,
        // Define o desvio de tempo permitido para a valida��o de tempo de vida do token
        ClockSkew = TimeSpan.Zero,
        // Recupera a audi�ncia v�lida da configura��o
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        // Recupera o emissor v�lido da configura��o
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        // Define a chave de assinatura do emissor para a valida��o do token
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// --- Fim das Configura��es de Seguran�a

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
