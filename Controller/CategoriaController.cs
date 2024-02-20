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
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappins;
using ApiCatalogo.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace ApiCatalogo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        // var produtos =_context.Produtos.AsNoTracking().ToList();
        public CategoriaController( IUnitOfWork unitOfWork)
        {
            
            _unitOfWork = unitOfWork;
        }

        // GET: api/Categoria
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> listCategorias()
        {
            var categorias = await _unitOfWork.CategoriaRepository.GetCategorias();
            var categoriasDTO = categorias.ToCategoriaDTOList();
            return Ok(categoriasDTO);
        }

        // GET: api/Categoria
        [HttpGet("Pagination")]
        public async Task<IActionResult> GetCategoriasPagination([FromQuery] CategoriaParameters categoriaParameters)
        {
            var categorias = await _unitOfWork.CategoriaRepository.GetAllCategoriasAsync(categoriaParameters);
           
            var metada = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };
            
            Response.Headers.Append("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(metada));
            
            return Ok(categorias.ToCategoriaDTOList());
        }

        // Neste método, o parâmetro name é vai ser utilizado pelo meu service para retornar a saudação
        [HttpGet("UsandoServices/{name}")]
        public ActionResult<string> GetSalvacaoTesteService([FromServices] IService service, string name)
        {
            return service.Saudacao(name);
        }

        // GET: api/Categoria/5
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoriaController(int id)
        {
            //Busco Categoria pelo Id no banco de dados
            var categoria = await _unitOfWork.CategoriaRepository.GetAsync(x => x.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }
            //devo reornar um DTO
            //transformo uma categoria em uma categoriaDTO
            var categoriaDTO = categoria.ToCategoriaDTO();
            //retorno a categoriaDTO
            return Ok(categoriaDTO);
        }

        // PUT: api/Categoria/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaDTO>> PutCategoria(int id, CategoriaDTO categoriaDTO)
        {
        
            //agorra transformo o DTO em uma categoria para envia-la para o banco de dados
            var categoria = categoriaDTO.CategoriaDTO();

            await _unitOfWork.CategoriaRepository.UpdateAsync(categoria);
            _unitOfWork.Commit();

            return NoContent(); // Atualização bem-sucedida, retorna um 204 No Content
        }

        // POST: api/Categoria
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> PostCategoria(CategoriaDTO categoriaDTO)
        {
            var categoria = new Categoria()
            {
                Nome = categoriaDTO.Nome,
                imagemUrl = categoriaDTO.imagemUrl
            };

            await _unitOfWork.CategoriaRepository.CreateAsync(categoria);
            _unitOfWork.Commit();

            return Ok(categoria.ToCategoriaDTO);
        }

        // DELETE: api/Categoria/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _unitOfWork.CategoriaRepository.GetAsync(x => x.CategoriaId==id);
            if (categoria == null)
            {
                return NotFound();
            }

            await _unitOfWork.CategoriaRepository.DeleteAsync(categoria);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("nome/{name}")]
        public async Task<ActionResult<CategoriaDTO>> CategoriaPorNome(string name)
        {
            var categoria = await _unitOfWork.CategoriaRepository.GetCategoriaPorNome(name);
            if (categoria == null)
            {
                return NotFound();
            }

            return Ok(categoria.ToCategoriaDTO);
        }
    }
}