using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Categoria> CreateCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return categoria;
        }

        public async Task<Categoria> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return categoria;
        }

        public async Task<Categoria> GetCategoria(int categoriaId)
        {
            return await _context.Categorias.FindAsync(categoriaId);
        }


        public async Task<Categoria> GetCategoriaPorNome(string nomeCategoria)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Nome == nomeCategoria);
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            return await _context.Categorias
                .Include(c => c.Produtos)  // Inclui os produtos relacionados a cada categoria
                .Where(c => c.CategoriaId <= 5)  // Filtra as categorias cujo CategoriaId é menor ou igual a 5
                .AsNoTracking()  // Indica que as entidades retornadas não precisam ser rastreadas
                .ToListAsync();  // Converte a consulta em uma lista assíncrona       
        }

        public async Task<Categoria> UpdateCategoria(Categoria categoria)
        {
            var categoriaBd = await _context.Categorias.FindAsync(categoria.CategoriaId);
           
            _context.Entry(categoriaBd).CurrentValues.SetValues(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }
    }
}
