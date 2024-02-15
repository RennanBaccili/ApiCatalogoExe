using ApiCatalogo.Context;
using ApiCatalogo.Exceptions;
using ApiCatalogo.Logging;
using ApiCatalogo.Repository;
using ApiCatalogo.Repository.IRepository;
using ApiCatalogo.Services;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Ativa os constroladores da API
builder.Services.AddControllers(options => options.Filters.Add(typeof(ApiExceptionFilter)))
                .AddJsonOptions(options =>
                { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

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
