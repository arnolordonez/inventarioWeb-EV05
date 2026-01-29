using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa la relación N:M entre Referencias, Tallas, Géneros y Telas.
    /// </summary>
    [Table("referencias_telas")]
    public class ReferenciaTela
    {
        // ==========================================================
        // 🔹 CLAVES PRIMARIAS COMPUESTAS
        // ==========================================================
        [Key, Column(Order = 0)]
        [Display(Name = "Referencia")]
        public int ID_Referencias { get; set; }

        [Key, Column(Order = 1)]
        [Display(Name = "Talla")]
        public int ID_Tallas { get; set; }

        [Key, Column(Order = 2)]
        [Display(Name = "Género")]
        public int ID_Genero { get; set; }

        [Key, Column(Order = 3)]
        [Display(Name = "Tela")]
        public int ID_Telas { get; set; }

        // ==========================================================
        // 🔹 CAMPO ADICIONAL
        // ==========================================================
        [StringLength(100)]
        [Display(Name = "Tipo de Género")]
        public string? TipoGenero { get; set; }

        // ==========================================================
        // 🔗 RELACIONES (FOREIGN KEYS)
        // ==========================================================
        [ForeignKey(nameof(ID_Referencias))]
        public virtual Referencia Referencia { get; set; } = null!;

        [ForeignKey(nameof(ID_Tallas))]
        public virtual Talla Talla { get; set; } = null!;

        [ForeignKey(nameof(ID_Genero))]
        public virtual Genero Genero { get; set; } = null!;

        [ForeignKey(nameof(ID_Telas))]
        public virtual Tela Tela { get; set; } = null!;
    }
}
