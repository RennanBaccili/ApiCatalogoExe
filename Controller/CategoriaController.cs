using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ApiCatalogo.Services;
using ApiCatalogo.Repository.IRepository;
using ApiCatalogo.Repository;

namespace ApiCatalogo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ICategoriaRepository _categoriaRepository;
        // var produtos =_context.Produtos.AsNoTracking().ToList();
        public CategoriaController(ICategoriaRepository CategoriaRepository)
        {
            _categoriaRepository = CategoriaRepository;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> listCategorias()
        {
            var categorias = await _categoriaRepository.GetCategorias();
            return Ok(categorias);
        }


        // Neste método, o parâmetro name é vai ser utilizado pelo meu service para retornar a saudação
        [HttpGet("UsandoServices/{name}")]
        public ActionResult<string> GetSalvacaoTesteService([FromServices] IService service, string name)
        {
            return service.Saudacao(name);
        }

        // GET: api/Categoria/5
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Categoria>> GetCategoriaController(int id)
        {
            var categoria = await _categoriaRepository.GetAsync(p => p.CategoriaId==id);
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        // PUT: api/Categoria/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }

            await _categoriaRepository.UpdateAsync(categoria);

            return NoContent(); // Atualização bem-sucedida, retorna um 204 No Content
        }

        // POST: api/Categoria
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            await _categoriaRepository.CreateAsync(categoria);
            return CreatedAtAction("GetCategoria", new { id = categoria.CategoriaId }, categoria);
        }

        // DELETE: api/Categoria/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _categoriaRepository.GetAsync(x => x.CategoriaId==id);
            if (categoria == null)
            {
                return NotFound();
            }

            await _categoriaRepository.DeleteAsync(categoria);

            return NoContent();
        }

        [HttpGet("nome/{name}")]
        public async Task<ActionResult<Categoria>> CategoriaPorNome(string name)
        {
            var categoria = await _categoriaRepository.GetCategoriaPorNome(name);
            if (categoria == null)
            {
                return NotFound();
            }

            return Ok(categoria);
        }

    }
}