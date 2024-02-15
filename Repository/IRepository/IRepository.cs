using System.Linq.Expressions;

namespace ApiCatalogo.Repository.IRepository
{
    // Definição de uma interface genérica chamada IRepository com um parâmetro de tipo T
    public interface IRepository<T>
    {
        // Método para obter todos os itens do repositório
        Task<IEnumerable<T>> GetAllAsync();

        // Método para obter um item específico com base em um predicado
        // O predicado é uma expressão lambda que define uma condição para o item que desejamos obter
        // O tipo de retorno é T?, que significa que o método pode retornar um item do tipo T ou null se não encontrar nenhum item
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }

}
