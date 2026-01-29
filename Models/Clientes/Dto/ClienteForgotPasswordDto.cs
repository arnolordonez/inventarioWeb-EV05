using System.ComponentModel.DataAnnotations;

namespace InventarioWEB.Models.Clientes.Dto
{
    public class ClienteForgotPasswordDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Correo { get; set; } = string.Empty;
    }
}
