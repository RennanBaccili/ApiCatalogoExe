using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _Context;
        public ProdutoController(AppDbContext Context)
        {
            _Context = Context;
        }


        [HttpGet]
        public ActionResult<Produto> GetAllProduto()
        {
            try
            {
                return Ok(_Context.Produtos.ToList());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter os produtos do banco de dados");
            }
        }

        [HttpGet("{id}", Name = "GetProduto")]
        public ActionResult<Produto> GetProdutoById(int id)
        {
            try
            {
                var produto = _Context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
                if (produto == null)
                {
                    return NotFound($"O produto com id = {id} não foi encontrado");
                }
                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter o produto do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult<Produto> PostProduct(Produto produto){  
            try
            {
                _Context.Produtos.Add(produto);
                _Context.SaveChanges();
                return CreatedAtRoute("GetProduto", new { id = produto.ProdutoId }, produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar um novo produto");
            }
        }
        [HttpPut("{id}")]
        public ActionResult<Produto> PutProduct(int id, Produto produto)
        {
            try
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
            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar atualizar o produto");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> DeleteProduct(int id)
        {
            try
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar remover o produto");
            }
        }
    }
}
