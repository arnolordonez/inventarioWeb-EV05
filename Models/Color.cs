using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWEB.Models
{
    [Table("colores")]
    public class Color
    {
        [Key]
        public int ID_Color { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
