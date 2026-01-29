using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    [Table("telas")]
    public class Tela
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Telas { get; set; }

        [Required, StringLength(150)]
        public string DescripTela { get; set; } = string.Empty;

        // 🔗 Relación N:M con Referencias y Tallas (a través de ReferenciaTela)
        public ICollection<ReferenciaTela> ReferenciasTelas { get; set; } = new List<ReferenciaTela>();
    }
}
