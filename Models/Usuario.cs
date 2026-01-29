using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using InventarioWEB;
namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa un usuario del sistema, vinculado a un rol.
    /// </summary>
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        public string HashContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor Salt es obligatorio")]
        [StringLength(255)]
        public string Salt { get; set; } = string.Empty;

        // 🔗 Relación con Rol
        [Required]
        public int IdRol { get; set; }

        [ForeignKey(nameof(IdRol))]
        public Rol Rol { get; set; } = null!;

        // Indica si el usuario está activo o no
        public bool Activo { get; set; } = true;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        public DateTime FechaUltimaActualizacion { get; set; } = DateTime.Now;
    }
}
