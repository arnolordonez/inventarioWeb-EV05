using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa los roles de usuario del sistema (por ejemplo: Administrador, Empleado, Cliente, etc.).
    /// </summary>
    [Table("roles")]
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRol { get; set; }

        [Required, StringLength(50)]
        public string NombreRol { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Descripcion { get; set; }

        // 🔗 Relación 1:N — un Rol puede estar asignado a varios Usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
