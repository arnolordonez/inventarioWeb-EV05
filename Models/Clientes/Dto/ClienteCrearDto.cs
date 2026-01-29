using System.ComponentModel.DataAnnotations;

namespace InventarioWEB.Models.Clientes.Dto
{
    public class ClienteCrearDto
    {
        [Required]
        public int ID_Cliente { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        public string? Telefono { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        public string? Direccion { get; set; }
        public string? CiudadMunicipio { get; set; }

        [Required]
        public string TipoCliente { get; set; }

        public string? Observaciones { get; set; }

        public bool VIP { get; set; }
    }
}
