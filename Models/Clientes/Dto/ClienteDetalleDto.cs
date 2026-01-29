namespace InventarioWEB.Models.Clientes.Dto
{
    public class ClienteDetalleDto
    {
        public int? ID_Cliente { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public string? Telefono { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? CiudadMunicipio { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string TipoCliente { get; set; } = "Minorista";
        public string? Observaciones { get; set; }

        public bool VIP { get; set; }
        public bool Activo { get; set; }
    }
}
