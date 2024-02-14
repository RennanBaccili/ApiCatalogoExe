using ApiCatalogo.Context;
using ApiCatalogo.Models;
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
        private readonly AppDbContext _Context;
        private readonly IConfiguration _Configuration;
        public ProdutoController(AppDbContext Context, IConfiguration Configuration)
        {
            _Context = Context;
            _Configuration = Configuration;
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
        public ActionResult<Produto> GetAllProduto()
        {
                return Ok(_Context.Produtos.ToList());
        }

        [HttpGet("{id}/find", Name = "GetProduto")]
        public ActionResult<Produto> GetProdutoById(int id)
        {
        
            throw new Exception("Erro ao tentar obter os produtos do banco de dados");
            var produto = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound($"O produto com id = {id} não foi encontrado");
            }
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult<Produto> PostProduct(Produto produto){  
            _Context.Produtos.Add(produto);
            _Context.SaveChanges();
            return CreatedAtRoute("GetProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id}")]
        public ActionResult<Produto> PutProduct(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest($"O produto com id = {id} não foi encontrado");
            }
            // Atualiza o produto
            _Context.Entry(produto).State = EntityState.Modified;
            _Context.SaveChanges();
            return Ok(produto);
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> DeleteProduct(int id)
        {
            var produto = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound($"O produto com id = {id} não foi encontrado");
            }
            _Context.Produtos.Remove(produto);
            _Context.SaveChanges();
            return Ok($"Produto com id = {id} foi removido");
        }
    }
}
