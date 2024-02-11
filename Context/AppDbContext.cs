using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Context
{
    public class AppDbContext : DbContext
    {

        // DbSet is a collection of entities that can be queried, such as tables in a database.
        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Produto>? Produtos { get; set; }

        // The DbContextOptions parameter is used to configure the context.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
    }
}
