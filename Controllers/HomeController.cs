using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventarioWEB.Models;
using System.Diagnostics;

using InventarioWEB;
namespace InventarioWEB.Controllers

{
    /// <summary>
    /// Controlador principal de la aplicación.
    /// Gestiona las vistas iniciales y el manejo básico de errores.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor con inyección de dependencias para el logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // ==========================================================
        // VISTA PRINCIPAL (Inicio)
        // ==========================================================
        public IActionResult Index()
        {
            return View();
        }

        // ==========================================================
        // VISTA DE PRIVACIDAD (ejemplo estándar de MVC)
        // ==========================================================
        public IActionResult Privacy()
        {
            return View();
        }

        // ==========================================================
        // PÁGINA DE ERROR GLOBAL
        // ==========================================================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Retorna la vista de error con el identificador de la solicitud
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
