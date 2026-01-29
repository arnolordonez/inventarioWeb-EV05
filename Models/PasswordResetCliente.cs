using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWEB.Models
{
    /// <summary>
    /// Representa una solicitud de restablecimiento de contraseña de un cliente.
    /// Esta clase pertenece a la base de datos MovimientoVentas.
    /// </summary>
    [Table("passwordresetsclientes")]
    public class PasswordResetCliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // ==========================================================
        // 🔗 RELACIÓN CON CLIENTE
        // ==========================================================
        [Required]
        [Column("ID_Cliente")]
        public int ID_Cliente { get; set; }

        [ForeignKey(nameof(ID_Cliente))]
        public Cliente Cliente { get; set; } = null!;

        // ==========================================================
        // 🔐 TOKEN Y VALIDACIÓN
        // ==========================================================
        [Required]
        [StringLength(255)] // CORREGIDO según tabla
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiraToken { get; set; }

        public bool Usado { get; set; } = false;

        [Required]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    }
}
