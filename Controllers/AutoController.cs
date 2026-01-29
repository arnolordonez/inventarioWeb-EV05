using Microsoft.AspNetCore.Mvc;
using InventarioWEB.Data;
using InventarioWEB.Models;
using InventarioWEB.Models.DTO;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace InventarioWEB.Controllers
{
    /// <summary>
    /// Controlador encargado de la autenticación y gestión de sesión
    /// de los usuarios del sistema (tabla Usuarios).
    /// </summary>
    public class AutoController : Controller
    {
        private readonly UsuariosDbContext _context;

        public AutoController(UsuariosDbContext context)
        {
            _context = context;
        }
                
        // ==========================================================
        // LOGIN (GET) - ahora con últimos 5 correos
        // ==========================================================
        [HttpGet]
        public IActionResult Login()
        {
            // Obtener los últimos 5 correos activos (más recientes)
            ViewBag.Correos = _context.Usuarios
                                      .Where(u => u.Activo)
                                      .OrderByDescending(u => u.FechaCreacion)
                                      .Select(u => u.Correo)
                                      .Take(5)
                                      .ToList();

            return View(new LoginRequest());
        }


        // ==========================================================
        // LOGIN (POST)
        // ==========================================================
        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == request.Correo);
            if (usuario != null && BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.HashContrasena))
            {
                HttpContext.Session.SetString("UsuarioID", usuario.IdUsuario.ToString());
                HttpContext.Session.SetString("UsuarioNombre", $"{usuario.Nombres} {usuario.Apellidos}");
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(request);
        }

        // ==========================================================
        // DASHBOARD
        // ==========================================================
        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioID")))
                return RedirectToAction("Login");

            ViewBag.NombreUsuario = HttpContext.Session.GetString("UsuarioNombre");
            return View();
        }

        // ==========================================================
        // REGISTRO DE NUEVO USUARIO
        // ==========================================================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            if (_context.Usuarios.Any(u => u.Correo == request.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(request);
            }

            var nuevoUsuario = new Usuario
            {
                Nombres = request.Nombres.Trim(),
                Apellidos = request.Apellidos.Trim(),
                Correo = request.Correo.Trim(),
                Salt = Guid.NewGuid().ToString(),
                HashContrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena.Trim()),
                IdRol = 1,
                Activo = true,
                FechaCreacion = DateTime.Now,
                FechaUltimaActualizacion = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ==========================================================
        // LOGOUT
        // ==========================================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================================================
        // FORGOT PASSWORD (GET)
        // ==========================================================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordRequest());
        }

        // ==========================================================
        // FORGOT PASSWORD (POST)
        // ==========================================================
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == request.Correo);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Correo no registrado.");
                return View(request);
            }

            // Generar token (ejemplo simple)
            var token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("ResetToken", token);
            HttpContext.Session.SetString("ResetUserId", usuario.IdUsuario.ToString());

            ViewBag.Mensaje = $"Token generado: {token} (en producción se enviaría por correo).";
            return View(request);
        }

        // ==========================================================
        // RESET PASSWORD (GET)
        // ==========================================================
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordRequest
            {
                Token = token ?? string.Empty
            };
            return View(model);
        }

        // ==========================================================
        // RESET PASSWORD (POST)
        // ==========================================================
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordRequest request)
        {
            if (!ModelState.IsValid) return View(request);

            var sessionToken = HttpContext.Session.GetString("ResetToken");
            var userIdStr = HttpContext.Session.GetString("ResetUserId");

            if (sessionToken != request.Token || string.IsNullOrEmpty(userIdStr))
            {
                ModelState.AddModelError("", "Token inválido o expirado.");
                return View(request);
            }

            int userId = int.Parse(userIdStr);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == userId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View(request);
            }

            // Actualizar contraseña
            usuario.HashContrasena = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena.Trim());
            usuario.FechaUltimaActualizacion = DateTime.Now;
            _context.SaveChanges();

            // Limpiar token de sesión
            HttpContext.Session.Remove("ResetToken");
            HttpContext.Session.Remove("ResetUserId");

            ViewBag.Mensaje = "Contraseña actualizada con éxito. Puede iniciar sesión.";
            return RedirectToAction("Login");
        }
    }

    // DTO adicional para recuperación de contraseña
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Correo { get; set; } = string.Empty;
    }
}
