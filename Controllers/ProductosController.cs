using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioWEB.Data;
using InventarioWEB.Models;
using InventarioWEB.ViewModels;

namespace InventarioWEB.Controllers
{
    public class ProductosController : Controller
    {
        private readonly MovimientoVentasDbContext _context;

        public ProductosController(MovimientoVentasDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: /Productos/Index
        // ============================================================
        public async Task<IActionResult> Index(string estado)
        {
            ViewBag.EstadosFiltro = new SelectList(new[]
            {
                new { Value = "", Text = "Todos" },
                new { Value = "A", Text = "Activos" },
                new { Value = "I", Text = "Inactivos" }
            }, "Value", "Text", estado);

            var query = _context.Productos
                .Include(p => p.Referencia).ThenInclude(r => r.Genero)
                .Include(p => p.Talla)
                .Include(p => p.Tela)
                .Include(p => p.Color)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                if (estado == "A") query = query.Where(p => p.Activo == true);
                if (estado == "I") query = query.Where(p => p.Activo == false);
            }

            var productos = await query.ToListAsync();
            return View(productos);
        }

        // ============================================================
        // GET: /Productos/Crear
        // ============================================================
        public async Task<IActionResult> Crear()
        {
            var model = new ProductoCreateViewModel
            {
                GenerosLista = await _context.Generos
                    .Select(x => new SelectListItem { Value = x.ID_Genero.ToString(), Text = x.DescripGenero })
                    .ToListAsync(),

                TelasLista = await _context.Telas
                    .Select(x => new SelectListItem { Value = x.ID_Telas.ToString(), Text = x.DescripTela })
                    .ToListAsync(),

                ColoresLista = await _context.Colores
                    .Select(x => new SelectListItem { Value = x.ID_Color.ToString(), Text = x.Nombre })
                    .ToListAsync(),

                ReferenciasLista = new List<SelectListItem>(),
                TallasLista = new List<SelectListItem>()
            };

            return View(model);
        }

        // ============================================================
        // POST: /Productos/Crear
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas(model);
                return View(model);
            }

            if (model.PrecioCosto <= 0)
                ModelState.AddModelError("PrecioCosto", "El precio de costo no puede ser 0.");

            if (model.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio de venta no puede ser 0.");

            if (model.Precio <= model.PrecioCosto)
                ModelState.AddModelError("Precio", "El precio de venta no puede ser menor o igual al costo.");

            if (!ModelState.IsValid)
            {
                await CargarListas(model);
                return View(model);
            }

            // =====================================================================
            // GENERAR LISTAS SEGÚN FILTROS
            // =====================================================================
            var referencias = model.ID_Referencias.HasValue
                ? new List<int> { model.ID_Referencias.Value }
                : await _context.Referencias
                    .Where(r => r.ID_Genero == model.ID_Genero)
                    .Select(r => r.ID_Referencias)
                    .ToListAsync();

            var tallas = model.ID_Tallas.HasValue
                ? new List<int> { model.ID_Tallas.Value }
                : await _context.Tallas
                    .Where(t => t.ID_Genero == model.ID_Genero)
                    .Select(t => t.ID_Tallas)
                    .ToListAsync();

            var colores = model.ID_Color.HasValue
                ? new List<int> { model.ID_Color.Value }
                : await _context.Colores
                    .Select(c => c.ID_Color)
                    .ToListAsync();

            if (model.ID_Telas == null)
            {
                ModelState.AddModelError("ID_Telas", "Debe seleccionar una tela.");
                await CargarListas(model);
                return View(model);
            }
            // =====================================================================
            // CREACIÓN / ACTUALIZACIÓN DE PRODUCTOS
            // =====================================================================
            foreach (var idRef in referencias)
            {
                foreach (var idTalla in tallas)
                {
                    foreach (var idColor in colores)
                    {
                        var existente = await _context.Productos.FirstOrDefaultAsync(p =>
                            p.ID_Referencias == idRef &&
                            p.ID_Tallas == idTalla &&
                            p.ID_Color == idColor &&
                            p.ID_Telas == model.ID_Telas
                        );

                        // ============================================
                        // Obtener nombres para generar Nombre del producto
                        // ============================================
                        var genero = await _context.Generos.FirstAsync(g => g.ID_Genero == model.ID_Genero);
                        var referencia = await _context.Referencias.FirstAsync(r => r.ID_Referencias == idRef);
                        var talla = await _context.Tallas.FirstAsync(t => t.ID_Tallas == idTalla);
                        var tela = await _context.Telas.FirstAsync(t => t.ID_Telas == model.ID_Telas);
                        var color = await _context.Colores.FirstAsync(c => c.ID_Color == idColor);

                        var nombreAuto = $"{referencia.DescripReferencia} {tela.DescripTela} {talla.DescripTalla} {genero.DescripGenero} {color.Nombre}";


                        // ==================================================
                        // SI YA EXISTE → ACTUALIZA
                        // ==================================================
                        if (existente != null)
                        {
                            existente.PrecioCosto = model.PrecioCosto;
                            existente.Precio = model.Precio;
                            existente.IVA_Porcentaje = model.IVA_Porcentaje;
                            existente.Stock = model.Stock;
                            existente.Nombre = nombreAuto;

                            _context.Productos.Update(existente);
                            await _context.SaveChangesAsync();
                            continue;
                        }


                        // ==================================================
                        // GENERAR NUEVA SECUENCIA PARA ID_Producto
                        // ==================================================
                        var ultimo = await _context.Productos
                            .OrderByDescending(p => p.ID_Producto)
                            .Select(p => p.ID_Producto)
                            .FirstOrDefaultAsync();

                        int numero = 0;

                        if (!string.IsNullOrEmpty(ultimo) && ultimo.Contains("_"))
                        {
                            int.TryParse(ultimo.Split('_')[0], out numero);
                        }

                        numero++;
                        var secuencia = numero.ToString("D6");

                        var nuevoId = $"{secuencia}_{nombreAuto}";


                        // ==================================================
                        // CREAR NUEVO PRODUCTO
                        // ==================================================
                        var producto = new Producto
                        {
                            ID_Producto = nuevoId,
                            Nombre = nombreAuto,
                            PrecioCosto = model.PrecioCosto,
                            Precio = model.Precio,
                            IVA_Porcentaje = model.IVA_Porcentaje,
                            Stock = model.Stock,
                            ID_Referencias = idRef,
                            ID_Tallas = idTalla,
                            ID_Telas = model.ID_Telas.Value,
                            ID_Color = idColor
                        };

                        _context.Productos.Add(producto);
                    }
                }
            }


            await _context.SaveChangesAsync();

            TempData["Success"] = "Productos creados o actualizados correctamente.";
            return RedirectToAction(nameof(Crear));
        }

        // ============================================================
        // MÉTODOS AJAX
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ObtenerReferenciasPorGenero(int idGenero)
        {
            var refs = await _context.Referencias
                .Where(r => r.ID_Genero == idGenero)
                .Select(r => new { r.ID_Referencias, Nombre = r.DescripReferencia })
                .ToListAsync();

            return Json(refs);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTallasPorGenero(int idGenero)
        {
            var tallas = await _context.Tallas
                .Where(t => t.ID_Genero == idGenero)
                .Select(t => new { t.ID_Tallas, Nombre = t.DescripTalla })
                .ToListAsync();

            return Json(tallas);
        }

        [HttpGet]
        public async Task<IActionResult> ReferenciasPorGenero(int id)
        {
            var refs = await _context.Referencias
                .Where(r => r.ID_Genero == id)
                .Select(r => new SelectListItem
                {
                    Value = r.ID_Referencias.ToString(),
                    Text = r.DescripReferencia
                })
                .ToListAsync();

            return Json(refs);
        }

        [HttpGet]
        public async Task<IActionResult> TallasPorReferencia(int id)
        {
            var idGenero = await _context.Referencias
                .Where(r => r.ID_Referencias == id)
                .Select(r => r.ID_Genero)
                .FirstOrDefaultAsync();

            var tallas = await _context.Tallas
                .Where(t => t.ID_Genero == idGenero)
                .Select(t => new SelectListItem
                {
                    Value = t.ID_Tallas.ToString(),
                    Text = t.DescripTalla
                })
                .ToListAsync();

            return Json(tallas);
        }

        // ============================================================
        // CargarListas
        // ============================================================
        private async Task CargarListas(ProductoCreateViewModel model)
        {
            model.GenerosLista = await _context.Generos
                .Select(x => new SelectListItem { Value = x.ID_Genero.ToString(), Text = x.DescripGenero })
                .ToListAsync();

            model.TelasLista = await _context.Telas
                .Select(x => new SelectListItem { Value = x.ID_Telas.ToString(), Text = x.DescripTela })
                .ToListAsync();

            model.ColoresLista = await _context.Colores
                .Select(x => new SelectListItem { Value = x.ID_Color.ToString(), Text = x.Nombre })
                .ToListAsync();

            model.ReferenciasLista = await _context.Referencias
                .Where(r => r.ID_Genero == model.ID_Genero)
                .Select(r => new SelectListItem { Value = r.ID_Referencias.ToString(), Text = r.DescripReferencia })
                .ToListAsync();

            model.TallasLista = await _context.Tallas
                .Where(t => t.ID_Genero == model.ID_Genero)
                .Select(t => new SelectListItem { Value = t.ID_Tallas.ToString(), Text = t.DescripTalla })
                .ToListAsync();
        }
        // ============================================================
        // ELIMINACIÓN LÓGICA
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.ID_Producto == id);

            if (producto == null)
                return NotFound();

            // 🔥 Eliminación lógica
            producto.Activo = false;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Editar(string id)
        {
            var producto = _context.Productos
                .Include(p => p.Referencia).ThenInclude(r => r.Genero)
                .Include(p => p.Talla)
                .Include(p => p.Tela)
                .Include(p => p.Color)             // ← NUEVO
                .FirstOrDefault(p => p.ID_Producto == id);

            if (producto == null)
                return NotFound();

            // listas para los dropdowns
            ViewBag.Referencias = new SelectList(_context.Referencias, "ID_Referencia", "DescripReferencia");
            ViewBag.Tallas = new SelectList(_context.Tallas, "ID_Talla", "DescripTalla");
            ViewBag.Telas = new SelectList(_context.Telas, "ID_Tela", "DescripTela");
            ViewBag.Colores = new SelectList(_context.Colores, "ID_Color", "Nombre");  // ← NUEVO

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                // recargar listas
                ViewBag.Referencias = new SelectList(_context.Referencias, "ID_Referencia", "DescripReferencia");
                ViewBag.Tallas = new SelectList(_context.Tallas, "ID_Talla", "DescripTalla");
                ViewBag.Telas = new SelectList(_context.Telas, "ID_Tela", "DescripTela");
                ViewBag.Colores = new SelectList(_context.Colores, "ID_Color", "Nombre");  // ← NUEVO
                return View(producto);
            }

            _context.Update(producto);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        // ============================================================
        // RECUPERAR PRODUCTO (RESTAURAR)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> Restaurar(string id)
        {
            if (id == null)
                return NotFound();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.ID_Producto == id);

            if (producto == null)
                return NotFound();

            // 🔥 Restauración lógica
            producto.Activo = true;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Producto restaurado correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}
