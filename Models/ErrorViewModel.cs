namespace InventarioWEB.Models
{
    /// <summary>
    /// Modelo que representa la información de error para la vista Error.cshtml.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Identificador único de la solicitud actual (para rastrear errores).
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indica si el RequestId debe mostrarse en la vista.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
