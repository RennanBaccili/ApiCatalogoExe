using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Runtime.CompilerServices;

namespace ApiCatalogo.Exceptions
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger; // Declaração de uma variável para registrar logs de exceção

        // Construtor da classe ApiExceptionFilter que recebe um ILogger<ApiExceptionFilter> como parâmetro
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger; // Atribuição do logger recebido como parâmetro à variável _logger
        }

        // Método da interface IExceptionFilter que é chamado quando uma exceção ocorre durante o processamento de uma requisição HTTP
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message); // Registro da exceção no log de erros

            context.ExceptionHandled = true; // Indica que a exceção foi tratada e não deve ser propagada

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Define o código de status da resposta HTTP como 500 (InternalServerError)

            // Cria um objeto ObjectResult contendo os detalhes do erro para retornar como resposta para o cliente
            context.Result = new ObjectResult(new ErrorDetails
            {
                StatusCode = context.HttpContext.Response.StatusCode, // Define o código de status da resposta como o mesmo código definido anteriormente
                Message = "Ocorreu um erro ao processar sua requisição", // Mensagem genérica de erro
                Trace = context.Exception.StackTrace // Pilha de chamadas da exceção para debug
            })
            {
                StatusCode = context.HttpContext.Response.StatusCode // Define o código de status da resposta novamente (repetição desnecessária)
            };
        }
    }
}
