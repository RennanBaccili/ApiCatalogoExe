using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {

        public CategoriaRepository(AppDbContext context) :base(context)
        {
            
        }

        public async Task<PagedList<Categoria>> GetAllCategoriasAsync(CategoriaParameters categoriasParams)
        {
            var catergorias = await GetAllAsync();

            // Ordena as categorias e converte para IQueryable.
            var catergoriasQuery = catergorias
                .OrderBy(c => c.CategoriaId)
                .AsQueryable(); // Certifique-se de chamar AsQueryable() aqui para converter IEnumerable para IQueryable.

            // Agora, você pode passar categoriasQuery para o método ToPagedList sem problemas.
            var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(catergoriasQuery, categoriasParams.PageNumber, categoriasParams.PageSize);
            return categoriasOrdenadas; // Retorne o resultado diretamente, não há necessidade de chamar ToPagedList novamente.
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
    }
}
