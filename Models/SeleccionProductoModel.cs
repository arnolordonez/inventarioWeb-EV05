using System.Collections.Generic;
using InventarioWEB.Models;

namespace InventarioWEB.Models
{
    /// <summary>
    /// Modelo para la selección de producto en la página SeleccionProducto.cshtml.
    /// Contiene listas de Géneros, Tallas, Telas y Colores para desplegar en los dropdowns.
    /// También almacena la selección del usuario.
    /// </summary>
    public class SeleccionProductoModel
    {
        // Listas para mostrar en los dropdowns
        public List<Genero> Generos { get; set; } = new List<Genero>();
        public List<Talla> Tallas { get; set; } = new List<Talla>();
        public List<Tela> Telas { get; set; } = new List<Tela>();
        public List<Color> Colores { get; set; } = new List<Color>();

        // Selección actual del usuario
        public int? ID_GeneroSeleccionado { get; set; }
        public int? ID_TallaSeleccionada { get; set; }
        public int? ID_TelaSeleccionada { get; set; }
        public int? ID_ColorSeleccionado { get; set; }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public SeleccionProductoModel() { }

        /// <summary>
        /// Constructor con inicialización de listas
        /// </summary>
        public SeleccionProductoModel(List<Genero> generos, List<Talla> tallas, List<Tela> telas, List<Color> colores)
        {
            Generos = generos;
            Tallas = tallas;
            Telas = telas;
            Colores = colores;
        }
    }
}
