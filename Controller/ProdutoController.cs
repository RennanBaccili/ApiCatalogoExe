using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository.IRepository;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProdutoController(
            IConfiguration Configuration,
            IProdutoRepository produtoRepository,
            IRepository<Produto> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _Configuration = Configuration;
            _produtoRepository = produtoRepository;
            _Repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAllProduto()
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();
            //var destionop = _mapper.Map<List<ProdutoDTO>>(produtos);
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet("Pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAllProduto([FromQuery] ProdutosParrameters produtosParameters)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParameters);
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return Ok(produtosDTO);
        }


        [HttpGet("{id}/find", Name = "GetProduto")]
        public ActionResult<ProdutoDTO> GetProdutoById(int id)
        {
        
            var produto = _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound($"O produto com id = {id} não foi encontrado");
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> PostProduct(ProdutoDTO produtoDTO)
        {
            var produto = _mapper.Map<Produto>(produtoDTO);
            var novoProduto = await _unitOfWork.ProdutoRepository.CreateAsync(produto);
            _unitOfWork.Commit();
            var novoProdutoDTO = _mapper.Map<ProdutoDTO>(novoProduto);
            return Ok(novoProdutoDTO);
        }

        [HttpPut]
        public async Task<ActionResult<ProdutoDTO>> PutProduct(ProdutoDTO produtoDTO)
        {
            var produto = _mapper.Map<Produto>(produtoDTO);
            await _unitOfWork.ProdutoRepository.UpdateAsync(produto);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> DeleteProduct(int id)
        {
            var entity = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
            await _unitOfWork.ProdutoRepository.DeleteAsync(entity);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPatch("{id}/UpdatePartinal")]
        public async Task<ActionResult<ProdutoDTOUpdateResponse>> PatchProduct(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchDocument)
        {
            if (patchDocument == null || id <=0)
            {
                return NotFound();
            }

            var entity = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
           
            var entityDTO = _mapper.Map<ProdutoDTOUpdateRequest>(entity);
            patchDocument.ApplyTo(entityDTO, ModelState);

            if (!TryValidateModel(entityDTO))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(entityDTO, entity);
            await _unitOfWork.ProdutoRepository.UpdateAsync(entity);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(entity));
        }
    }
}
