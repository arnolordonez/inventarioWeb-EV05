using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa el género de los productos o referencias (por ejemplo: Masculino, Femenino, Unisex).
    /// </summary>
    [Table("genero")]
    public class Genero
    {
        [Key]
        public int ID_Genero { get; set; }

        [Required]
        [StringLength(100)]
        public string DescripGenero { get; set; } = string.Empty;

        // 🔗 Relaciones 1:N con Tallas y Referencias
        public ICollection<Talla> Tallas { get; set; } = new List<Talla>();
        public ICollection<Referencia> Referencias { get; set; } = new List<Referencia>();

        // 🔗 Relación N:M con Telas a través de la tabla intermedia ReferenciasTelas
        public ICollection<ReferenciaTela> ReferenciasTelas { get; set; } = new List<ReferenciaTela>();
    }
}
