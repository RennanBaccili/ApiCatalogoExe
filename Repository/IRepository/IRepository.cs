using System.Linq.Expressions;

namespace ApiCatalogo.Repository.IRepository
{
    // Definição de uma interface genérica chamada IRepository com um parâmetro de tipo T
    public interface IRepository<T>
    {
        // Método para obter todos os itens do repositório
        IEnumerable<T> GetAll();

        // Método para obter um item específico com base em um predicado
        // O predicado é uma expressão lambda que define uma condição para o item que desejamos obter
        // O tipo de retorno é T?, que significa que o método pode retornar um item do tipo T ou null se não encontrar nenhum item
        T? Get(Expression<Func<T, bool>> predicate);

        T Create(T entity);
        T Update(T entity); 
        T Delete(T entity);
    }
}
