using ApiCatalogo.Models;

namespace ApiCatalogo.Repository.IRepository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<Categoria> GetCategoriaPorNome(string nomeCategoria);
        Task<IEnumerable<Categoria>> GetCategorias();
    }
}
