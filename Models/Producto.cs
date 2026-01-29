using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWEB.Models
{
    [Table("producto")]  // Nombre real de la tabla en la base de datos
    public class Producto
    {
        // ----------------------------------------------------------
        // PRIMARY KEY → Generado como GUID (string)
        // ----------------------------------------------------------
        [Key]
        [Column("ID_Producto")]
        public string ID_Producto { get; set; } = string.Empty;
        // Se asigna en el controlador mediante: Guid.NewGuid().ToString()

        // ----------------------------------------------------------
        // CAMPOS BÁSICOS
        // ----------------------------------------------------------

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;
        // Nombre auto-generado basado en: REFERENCIA + TELA + TALLA + GÉNERO + COLOR

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioCosto { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal IVA_Porcentaje { get; set; }

        // ----------------------------------------------------------
        // ELIMINACIÓN LÓGICA
        // ----------------------------------------------------------
        public bool Activo { get; set; } = true;
        // true  → producto visible  
        // false → producto eliminado lógicamente (NO se borra de BD)

        // ----------------------------------------------------------
        // CLAVES FORÁNEAS
        // ----------------------------------------------------------

        [Required]
        public int ID_Tallas { get; set; }

        [Required]
        public int ID_Referencias { get; set; }

        [Required]
        public int ID_Telas { get; set; }

        [Required]
        public int ID_Color { get; set; }

        // ----------------------------------------------------------
        // NAVEGACIÓN (JOIN EF CORE)
        // ----------------------------------------------------------
        [ForeignKey(nameof(ID_Tallas))]
        public Talla? Talla { get; set; }

        [ForeignKey(nameof(ID_Referencias))]
        public Referencia? Referencia { get; set; }

        [ForeignKey(nameof(ID_Telas))]
        public Tela? Tela { get; set; }

        [ForeignKey(nameof(ID_Color))]
        public Color? Color { get; set; }
    }
}
