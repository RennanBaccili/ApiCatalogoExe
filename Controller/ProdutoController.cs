using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace ApiCatalogo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IRepository<Produto> _Repository;
        private readonly IConfiguration _Configuration;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoController(IConfiguration Configuration, IProdutoRepository produtoRepository, IRepository<Produto> repository)
        {
            _Configuration = Configuration;
            _produtoRepository = produtoRepository;
            _Repository = repository;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public string GetConfigurationAppJson()
        {
            var valor1 = _Configuration["chave1"];
            var valor2 = _Configuration["chave2"];
            var valor3 = _Configuration["secao:chave3"];
            var valor4 = _Configuration["secao:chave4"];

            return $"Valor1: {valor1}, Valor2: {valor2}, Valor3: {valor3}, Valor4: {valor4}";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAllProduto()
        {
            var produtos = await _Repository.GetAllAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}/find", Name = "GetProduto")]
        public ActionResult<Produto> GetProdutoById(int id)
        {
        
            var produto = _Repository.GetAsync(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound($"O produto com id = {id} não foi encontrado");
            }
            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduct(Produto produto)
        {
            var novoProduto = await _Repository.CreateAsync(produto);
            return Ok(novoProduto);
        }

        [HttpPut]
        public async Task<ActionResult<Produto>> PutProduct(Produto produto)
        {
            await _Repository.UpdateAsync(produto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Produto>> DeleteProduct(int id)
        {
            var entity = await _Repository.GetAsync(p => p.ProdutoId == id);
            await _Repository.DeleteAsync(entity);
            return NoContent();
        }
    }
}
