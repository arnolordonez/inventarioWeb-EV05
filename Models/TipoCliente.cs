using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa los tipos de clientes en el sistema de ventas.
    /// Ejemplos: Mayorista, Minorista.
    /// </summary>
    [Table("tipocliente")]
    public class TipoCliente
    {
        [Key]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }
    }
}
