using ApiCatalogo.Models;

namespace ApiCatalogo.Repository.IRepository
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetCategorias();
        Task<Categoria> GetCategoria(int categoriaId);
        Task<Categoria> GetCategoriaPorNome(string nomeCategoria);
        Task<Categoria> CreateCategoria(Categoria categoria);
        Task<Categoria> UpdateCategoria(Categoria categoria);
        Task<Categoria> DeleteCategoria(int id);
    }
}
