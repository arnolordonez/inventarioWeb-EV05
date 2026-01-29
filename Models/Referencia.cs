using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa una referencia de producto (modelo o diseño), asociada a un género.
    /// </summary>
    [Table("referencias")]
    public class Referencia
    {
        [Key]
        public int ID_Referencias { get; set; }

        [Required, StringLength(150)]
        public string DescripReferencia { get; set; } = string.Empty;

        // ==========================================================
        // 🔗 RELACIÓN CON GÉNERO (N:1)
        // ==========================================================
        [Required]
        public int ID_Genero { get; set; }

        [ForeignKey(nameof(ID_Genero))]
        public Genero Genero { get; set; } = null!;

        // ==========================================================
        // 🔗 RELACIÓN N:M CON REFERENCIA_TELAS
        // ==========================================================
        public ICollection<ReferenciaTela> ReferenciasTelas { get; set; } = new List<ReferenciaTela>();
    }
}
