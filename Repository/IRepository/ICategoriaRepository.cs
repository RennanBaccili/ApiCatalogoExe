using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repository.IRepository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<Categoria> GetCategoriaPorNome(string nomeCategoria);
        Task<IEnumerable<Categoria>> GetCategorias();

        Task<PagedList<Categoria>> GetAllCategoriasAsync(CategoriaParameters categoriaParameters);
    }
}
