using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventarioWEB.ViewModels
{
    public class ProductoCreateViewModel
    {
        // ----------------------------------------------------
        // DATOS BÁSICOS
        // ----------------------------------------------------

        [Required(ErrorMessage = "Seleccione un género.")]
        public int? ID_Genero { get; set; }

        [Required(ErrorMessage = "Seleccione una referencia.")]
        public int? ID_Referencias { get; set; }

        [Required(ErrorMessage = "Seleccione una talla.")]
        public int? ID_Tallas { get; set; }

        [Required(ErrorMessage = "Seleccione una tela.")]
        public int? ID_Telas { get; set; }

        [Required(ErrorMessage = "Seleccione un color.")]
        public int? ID_Color { get; set; }

        // ----------------------------------------------------
        // NUMÉRICOS
        // ----------------------------------------------------

        [Required(ErrorMessage = "Ingrese el precio de costo.")]
        [Range(1, 999999999, ErrorMessage = "El precio de costo debe ser mayor que 0.")]
        public decimal PrecioCosto { get; set; }

        [Required(ErrorMessage = "Ingrese el precio de venta.")]
        [Range(1, 999999999, ErrorMessage = "El precio de venta debe ser mayor que 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Ingrese el IVA.")]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0 y 100.")]
        public decimal IVA_Porcentaje { get; set; }

        [Required(ErrorMessage = "Ingrese el stock.")]
        [Range(1, 999999, ErrorMessage = "El stock debe ser mínimo 1.")]
        public int Stock { get; set; }

        // ----------------------------------------------------
        // CAMPOS GENERADOS AUTOMÁTICAMENTE (NO SE DIGITAN)
        // ----------------------------------------------------

        public string? Nombre { get; set; }

        public string? ID_Producto { get; set; }

        // ----------------------------------------------------
        // LISTAS PARA LOS COMBOS
        // ----------------------------------------------------

        public IEnumerable<SelectListItem>? GenerosLista { get; set; }
        public IEnumerable<SelectListItem>? ReferenciasLista { get; set; }
        public IEnumerable<SelectListItem>? TallasLista { get; set; }
        public IEnumerable<SelectListItem>? TelasLista { get; set; }
        public IEnumerable<SelectListItem>? ColoresLista { get; set; }
    }
}
