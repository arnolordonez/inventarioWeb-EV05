using Microsoft.AspNetCore.Mvc;
using InventarioWEB.Data;
using System;
using System.Linq;
using InventarioWEB;
namespace InventarioWEB.Controllers
{
    /// <summary>
    /// Controlador de prueba para verificar la conexión con la base de datos MySQL.
    /// Permite confirmar si el contexto "UsuariosDbContext" está funcionando correctamente.
    /// </summary>
    public class TestController : Controller
    {
        private readonly UsuariosDbContext _context;

        // Inyección de dependencias del contexto de base de datos
        public TestController(UsuariosDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // TEST DE CONEXIÓN
        // ==========================================================
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                // Intenta contar los registros de la tabla "usuarios"
                var total = _context.Usuarios.Count();
                return Content($"✅ Conexión correcta con la base de datos MySQL.\n" +
                               $"Usuarios registrados en la tabla: {total}");
            }
            catch (Exception ex)
            {
                // Si falla, muestra el error para diagnóstico
                return Content($"❌ ERROR al conectar con la base de datos MySQL:\n{ex.Message}");
            }
        }
    }
}
