using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ApiCatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto> ,IProdutoRepository
    {
        
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Produto>> GetProdutosAsync(ProdutosParrameters produtosParameters)
        {
            // Busca assíncrona dos produtos
            var produtos = await GetAllAsync();

            // Ordenação e paginação dos produtos
            var produtosOrdenadosPaginados = produtos
                .OrderBy(p => p.Nome) // Aqui, assegura-se que 'p' refere-se ao produto
                .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
                .Take(produtosParameters.PageSize);

            return produtosOrdenadosPaginados;
        }
    }
}
