using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioWEB.Models
{
    // =========================================================
    // Developer Notes:
    // Clase Cliente: Representa la tabla "cliente" en la BD.
    // Este modelo se usa en CRUD en ClientesController.
    //
    // Reglas importantes:
    // - ID_Cliente es nullable para evitar mostrar 0 en formularios nuevos.
    // - VIP se determina según TipoCliente (Minorista/Mayorista) en la lógica del controller.
    // - Activo indica si el cliente está activo o ha sido "eliminado" (soft delete).
    // - HashContrasena y Salt se generan automáticamente al crear un cliente.
    // - Pedidos es la relación 1:N con la entidad Pedido (si aplica).
    // =========================================================
    [Table("cliente")]
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Cédula")]
        public int? ID_Cliente { get; set; }  // nullable para no mostrar 0 en formularios

        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(30)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required, StringLength(150)]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        [Display(Name = "Ciudad / Municipio")]
        public string? CiudadMunicipio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required, StringLength(50)]
        [Display(Name = "Tipo de cliente")]
        public string TipoCliente { get; set; } = "Minorista";

        [Column(TypeName = "text")]
        public string? Observaciones { get; set; }

        [Column(TypeName = "tinyint(1)")]
        public bool VIP { get; set; } = true;

        [Column(TypeName = "tinyint(1)")]
        public bool Activo { get; set; } = true;

        [Required, StringLength(255)]
        public string HashContrasena { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Salt { get; set; } = string.Empty;

        // Relación 1:N con Pedidos
        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
