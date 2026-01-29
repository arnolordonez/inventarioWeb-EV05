using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using InventarioWEB.Models;

namespace InventarioWEB.Controllers
{
    public class ReferenciaController : Controller
    {
        // ==========================================================
        // 🔗 CLIENTE HTTP PARA CONSUMIR LA API
        // ==========================================================
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReferenciaController> _logger;

        public ReferenciaController(IHttpClientFactory httpClientFactory, ILogger<ReferenciaController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiInventario"); // Cliente configurado en Program.cs
            _logger = logger;
        }

        // ==========================================================
        // 📋 LISTAR REFERENCIAS (GET /api/referencias)
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                var referencias = await _httpClient.GetFromJsonAsync<List<Referencia>>("referencias");
                return View(referencias ?? new List<Referencia>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las referencias desde la API.");
                TempData["Error"] = "No se pudieron cargar las referencias desde el servidor.";
                return View(new List<Referencia>());
            }
        }

        // ==========================================================
        // 📄 DETALLES DE UNA REFERENCIA (GET /api/referencias/{id})
        // ==========================================================
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var referencia = await _httpClient.GetFromJsonAsync<Referencia>($"referencias/{id}");
                if (referencia == null)
                    return NotFound();

                return View(referencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la referencia con ID {id}");
                TempData["Error"] = "Error al cargar los detalles de la referencia.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==========================================================
        // 🧩 CREAR NUEVA REFERENCIA (GET)
        // ==========================================================
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================================
        // 🧩 CREAR NUEVA REFERENCIA (POST /api/referencias)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Referencia referencia)
        {
            if (!ModelState.IsValid)
                return View(referencia);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("referencias", referencia);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Referencia creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No se pudo crear la referencia. Verifica los datos.";
                return View(referencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la referencia.");
                TempData["Error"] = "Ocurrió un error al intentar crear la referencia.";
                return View(referencia);
            }
        }

        // ==========================================================
        // ✏️ EDITAR REFERENCIA (GET /api/referencias/{id})
        // ==========================================================
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var referencia = await _httpClient.GetFromJsonAsync<Referencia>($"referencias/{id}");
                if (referencia == null)
                    return NotFound();

                return View(referencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la referencia {id} para edición.");
                TempData["Error"] = "Error al cargar la referencia para editar.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==========================================================
        // ✏️ EDITAR REFERENCIA (PUT /api/referencias/{id})
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Referencia referencia)
        {
            if (id != referencia.ID_Referencias)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(referencia);

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"referencias/{id}", referencia);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Referencia actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "No se pudo actualizar la referencia.";
                return View(referencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la referencia con ID {id}");
                TempData["Error"] = "Error durante la actualización de la referencia.";
                return View(referencia);
            }
        }

        // ==========================================================
        // 🗑️ ELIMINAR REFERENCIA (GET /api/referencias/{id})
        // ==========================================================
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var referencia = await _httpClient.GetFromJsonAsync<Referencia>($"referencias/{id}");
                if (referencia == null)
                    return NotFound();

                return View(referencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la referencia {id} para eliminar.");
                TempData["Error"] = "Error al cargar la referencia para eliminar.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==========================================================
        // 🗑️ ELIMINAR REFERENCIA (DELETE /api/referencias/{id})
        // ==========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"referencias/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Referencia eliminada correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar la referencia.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la referencia con ID {id}");
                TempData["Error"] = "Error durante la eliminación de la referencia.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==========================================================
        // 🔗 OBTENER TELAS RELACIONADAS CON UNA REFERENCIA
        // ==========================================================
        public async Task<IActionResult> TelasPorReferencia(int idReferencia)
        {
            try
            {
                var relaciones = await _httpClient.GetFromJsonAsync<List<ReferenciaTela>>($"referencias_telas?referenciaId={idReferencia}");
                return View(relaciones ?? new List<ReferenciaTela>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las telas de la referencia {idReferencia}");
                TempData["Error"] = "No se pudieron cargar las telas asociadas.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==========================================================
        // 📏 OBTENER TALLAS POR GÉNERO (AJAX)
        // ==========================================================
        [HttpGet]
        public async Task<JsonResult> TallasPorGenero(int idGenero)
        {
            try
            {
                var todasTallas = await _httpClient.GetFromJsonAsync<List<Talla>>("tallas");
                var tallasFiltradas = todasTallas?
                    .Where(t => t.ID_Genero == idGenero)
                    .Select(t => new { id = t.ID_Tallas, descripcion = t.DescripTalla })
                    .ToList();
                return Json(tallasFiltradas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener tallas para el género {idGenero}");
                return Json(new List<object>());
            }
        }

        // ==========================================================
        // 🧵 OBTENER TELAS POR TALLA Y GÉNERO (AJAX)
        // ==========================================================
        [HttpGet]
        public async Task<JsonResult> TelasPorTalla(int idTalla, int idGenero)
        {
            try
            {
                var relaciones = await _httpClient.GetFromJsonAsync<List<ReferenciaTela>>("referencias_telas");
                if (relaciones == null)
                    return Json(new List<object>());

                var telasIds = relaciones
                    .Where(r => r.ID_Tallas == idTalla && r.ID_Genero == idGenero)
                    .Select(r => r.ID_Telas)
                    .Distinct()
                    .ToList();

                var todasTelas = await _httpClient.GetFromJsonAsync<List<Tela>>("telas");
                var telasFiltradas = todasTelas?
                    .Where(t => telasIds.Contains(t.ID_Telas))
                    .Select(t => new { id = t.ID_Telas, descripcion = t.DescripTela })
                    .ToList();

                return Json(telasFiltradas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener telas para la talla {idTalla} y género {idGenero}");
                return Json(new List<object>());
            }
        }

        // ==========================================================
        // 🎨 OBTENER COLORES DISPONIBLES (AJAX)
        // ==========================================================
        [HttpGet]
        public async Task<JsonResult> Colores()
        {
            try
            {
                var colores = await _httpClient.GetFromJsonAsync<List<Color>>("colores");
                var lista = colores?.Select(c => new { id = c.ID_Color, descripcion = c.Nombre }).ToList();
                return Json(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener colores");
                return Json(new List<object>());
            }
        }
    }
}
