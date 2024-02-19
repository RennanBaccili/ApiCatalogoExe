namespace ApiCatalogo.Pagination;

// Define uma classe PagedList genérica que herda de List<T>, onde T é um tipo de classe.
public class PagedList<T> : List<T> where T : class
{
    // Propriedade que armazena o número da página atual.
    public int CurrentPage { get; set; }
    // Propriedade que armazena o total de páginas disponíveis.
    public int TotalPages { get; set; }
    // Propriedade que armazena o número de itens por página.
    public int PageSize { get; set; }
    // Propriedade que armazena a contagem total de itens disponíveis.
    public int TotalCount { get; set; }

    // Propriedade somente leitura que indica se há uma página anterior.
    public bool HasPrevious => (CurrentPage > 1);
    // Propriedade somente leitura que indica se há uma próxima página.
    public bool HasNext => (CurrentPage < TotalPages);

    // Construtor que inicializa uma instância de PagedList com os itens e informações de paginação.
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count; // Define o total de itens.
        PageSize = pageSize; // Define o tamanho da página.
        CurrentPage = pageNumber; // Define o número da página atual.
                                  // Calcula o total de páginas, arredondando para cima o resultado da divisão entre o total de itens e o tamanho da página.
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        // Adiciona os itens à lista atual, herdada de List<T>.
        AddRange(items);
    }
    //Essa função é responsável por criar uma instância de PagedList com os itens e informações de paginação.
    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        // Calcula o total de itens.
        var count = source.Count();
        // Obtém os itens da página atual.
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        // Inicializa uma nova instância de PagedList com os itens e informações de paginação.
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
