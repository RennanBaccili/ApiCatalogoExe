using ApiCatalogo.Context;
using ApiCatalogo.Repository.IRepository;

namespace ApiCatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProdutoRepository _produtoRepository;
        public ICategoriaRepository _categoriaRepository;
        public AppDbContext _context;

        public UnitOfWork(AppDbContext context) // no construtor, injetamos o contexto, mas não o ProdutoRepository e o CategoriaRepository
        {
            _context = context;
        }


        // Em vez de buscar as instancias no construtor, vrificamos se elas já foram instanciadas. Se sim, retornamos a instância já existente. Se não, instanciamos e retornamos a instância.
        public IProdutoRepository ProdutoRepository
        {
            get
            {
                    return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);
            }
        }
        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                    return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context);
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
               _context.Dispose();
        }
    }
    
}
