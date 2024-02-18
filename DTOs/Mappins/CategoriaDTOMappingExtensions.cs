using ApiCatalogo.Models;

namespace ApiCatalogo.DTOs.Mappins
{
    public static class CategoriaDTOMappingExtensions
    {
        public static CategoriaDTO ToCategoriaDTO(this Categoria categoria)
        {
            if(categoria == null)
            {
                return null;
            }
            return new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                imagemUrl = categoria.imagemUrl
            };
        }
        public static Categoria? CategoriaDTO(this CategoriaDTO categoriaDTO)
        {
            if(categoriaDTO == null)
            {
                return null;
            }
            return new Categoria
            {
                CategoriaId = categoriaDTO.CategoriaId,
                Nome = categoriaDTO.Nome,
                imagemUrl = categoriaDTO.imagemUrl
            };
        }
        public static IEnumerable<CategoriaDTO> ToCategoriaDTOList(this IEnumerable<Categoria> categorias)
        {
            if(categorias == null)
            {
                return new List<CategoriaDTO>();
            }
            return categorias.Select(c => c.ToCategoriaDTO());
        }
    }
}
