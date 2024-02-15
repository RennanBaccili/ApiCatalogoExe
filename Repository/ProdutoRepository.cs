using ApiCatalogo.Context;
using ApiCatalogo.Models;
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

    }
}
