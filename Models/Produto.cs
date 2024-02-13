﻿using ApiCatalogo.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiCatalogo.Models;

[Table("Produtos")]
public class Produto : IValidatableObject
{

    [Key]
    public int ProdutoId { get; set; }

    [Required]
    [StringLength(80)]
    [PrimeiraLetraMaiuscula]
    public string? Nome { get; set; }

    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    //foreign key
    [ForeignKey("CategoriaId")]
    public int CategoriaId { get; set; }
    // Navigation property
    [JsonIgnore]
    public Categoria? Categoria { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       if(!string.IsNullOrEmpty(this.Nome))
        {
           var primeiraLetra = this.Nome[0].ToString();
           if (primeiraLetra != primeiraLetra.ToUpper())
            {
               yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula", new string[] { nameof(Nome) });
           }
       }
       if(this.Estoque <= 0)
        {
           yield return new ValidationResult("O estoque deve ser maior que zero", new string[] { nameof(Estoque) });
       }
    }
}
