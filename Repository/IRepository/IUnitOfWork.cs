namespace ApiCatalogo.Repository.IRepository;

public interface IUnitOfWork
{
    IProdutoRepository ProdutoRepository { get; }
    ICategoriaRepository CategoriaRepository { get; }
    void Commit();
}
