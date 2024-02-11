using System.Collections.ObjectModel;

namespace ApiCatalogo.Models;

public class Categoria
{
    // The constructor initializes the collection navigation property.
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
    public int CategoriaId { get; set; }

    public string? Nome { get; set; }

    public string? imagemUrl { get; set; }
    // Navigation property, one-to-many relationship
    public ICollection<Produto>? Produtos { get; set; }
}
