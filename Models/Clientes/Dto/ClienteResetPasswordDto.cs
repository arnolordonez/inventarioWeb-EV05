using System.ComponentModel.DataAnnotations;

namespace InventarioWEB.Models.Clientes.Dto
{
    public class ClienteResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres.")]
        public string NuevaContrasena { get; set; } = string.Empty;
    }
}
