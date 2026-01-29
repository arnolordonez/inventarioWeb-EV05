using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    [Table("tallas")]
    public class Talla
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Tallas { get; set; }

        [Required, StringLength(100)]
        public string DescripTalla { get; set; } = string.Empty;

        [Required]
        public int ID_Genero { get; set; }

        [ForeignKey("ID_Genero")]
        public Genero Genero { get; set; } = null!;

        // 🔗 Relación N:M con Referencias y Telas (a través de la tabla referencias_telas)
        public ICollection<ReferenciaTela> ReferenciasTelas { get; set; } = new List<ReferenciaTela>();
    }
}
