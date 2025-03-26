using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O Nome é Obrigatório")]
        [StringLength(80, ErrorMessage = "O nome deve ter entre 5 e 20 caracteres ", MinimumLength = 5)]
        //[PrimeiraLetraMaiuscula]
        public string? Nome { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Adescrição deve ter no máximo {1} caracteres")]
        public string? Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string? ImagemUrl { get; set; }

        public int CategoriaId { get; set; }
    }
}
