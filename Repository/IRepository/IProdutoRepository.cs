using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repository.IRepository;

public interface IProdutoRepository : IRepository<Produto>
{
     Task<IEnumerable<Produto>> GetProdutosAsync(ProdutosParrameters produtosParameters);
}
