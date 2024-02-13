namespace ApiCatalogo.Services
{
    public class Service : IService
    {
        public string Saudacao(string nome)
        {
            return $"Bem-vindo, {nome}! \n\n {DateTime.UtcNow}";
        }
    }
}
