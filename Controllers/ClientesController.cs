using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioWEB.Data;
using InventarioWEB.Models;
using X.PagedList;

namespace InventarioWEB.Controllers
{
    public class ClientesController : Controller
    {
        private readonly MovimientoVentasDbContext _context;

        public ClientesController(MovimientoVentasDbContext context)
        {
            _context = context;
        }

        // LISTADO CON PAGINACIÓN Y FILTRO
        public async Task<IActionResult> Index(int? page, bool soloEliminados = false)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            IQueryable<Cliente> query = _context.Clientes;

            query = soloEliminados
                ? query.Where(c => !c.Activo)
                : query.Where(c => c.Activo);

            var clientes = await query
                .OrderBy(c => c.Nombre)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewBag.SoloEliminados = soloEliminados;

            return View(clientes);
        }

        // DETALLES DEL CLIENTE
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ID_Cliente == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // CREAR CLIENTE (GET)
        public async Task<IActionResult> Create()
        {
            // Corregido: Generar SelectListItem para asp-items
            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Nombre
                })
                .ToListAsync();

            return View(new Cliente
            {
                FechaRegistro = DateTime.Now,
                VIP = true,
                Activo = true
            });
        }

        // CREAR CLIENTE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente model)
        {
            // Corregido: Generar SelectListItem para asp-items
            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Nombre
                })
                .ToListAsync();

            // VALIDACIÓN DE CÉDULA
            string idText = model.ID_Cliente.ToString();
            if (string.IsNullOrWhiteSpace(idText) || idText.Length < 6 || idText.Length > 10 || idText.StartsWith("0"))
            {
                ModelState.AddModelError(nameof(model.ID_Cliente),
                    "La cédula debe tener entre 6 y 10 dígitos y no puede empezar con 0.");
            }
            else if (await _context.Clientes.AnyAsync(c => c.ID_Cliente == model.ID_Cliente))
            {
                ModelState.AddModelError(nameof(model.ID_Cliente), "Ya existe un cliente con esa cédula.");
            }

            // VALIDACIÓN DE CAMPOS OBLIGATORIOS
            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Apellido))
                ModelState.AddModelError(nameof(model.Apellido), "El apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Correo))
                ModelState.AddModelError(nameof(model.Correo), "El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Telefono))
                ModelState.AddModelError(nameof(model.Telefono), "El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Direccion))
                ModelState.AddModelError(nameof(model.Direccion), "La dirección es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.CiudadMunicipio))
                ModelState.AddModelError(nameof(model.CiudadMunicipio), "La ciudad o municipio es obligatoria.");

            if (!await _context.TipoCliente.AnyAsync(t => t.Nombre == model.TipoCliente))
                ModelState.AddModelError(nameof(model.TipoCliente), "Tipo de cliente inválido.");

            if (!ModelState.IsValid)
                return View(model);

            // CAMPOS AUTOMÁTICOS
            model.FechaRegistro = DateTime.Now;
            model.Activo = true;

            if (string.IsNullOrWhiteSpace(model.HashContrasena))
            {
                model.Salt = Guid.NewGuid().ToString();
                model.HashContrasena = BCrypt.Net.BCrypt.HashPassword("default123" + model.Salt);
            }

            _context.Clientes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // EDITAR CLIENTE (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Nombre
                })
                .ToListAsync();

            return View(cliente);
        }

        // EDITAR CLIENTE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente model)
        {
            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => new SelectListItem
                {
                    Text = t.Nombre,
                    Value = t.Nombre
                })
                .ToListAsync();

            if (id != model.ID_Cliente)
                return BadRequest();

            // Validaciones
            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Apellido))
                ModelState.AddModelError(nameof(model.Apellido), "El apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Correo))
                ModelState.AddModelError(nameof(model.Correo), "El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Telefono))
                ModelState.AddModelError(nameof(model.Telefono), "El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Direccion))
                ModelState.AddModelError(nameof(model.Direccion), "La dirección es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.CiudadMunicipio))
                ModelState.AddModelError(nameof(model.CiudadMunicipio), "La ciudad o municipio es obligatoria.");

            if (!await _context.TipoCliente.AnyAsync(t => t.Nombre == model.TipoCliente))
                ModelState.AddModelError(nameof(model.TipoCliente), "Tipo de cliente inválido.");

            if (!ModelState.IsValid)
                return View(model);

            var clienteDb = await _context.Clientes.FindAsync(id);
            if (clienteDb == null) return NotFound();

            // Actualizar campos
            clienteDb.Nombre = model.Nombre;
            clienteDb.Apellido = model.Apellido;
            clienteDb.Telefono = model.Telefono;
            clienteDb.Correo = model.Correo;
            clienteDb.Direccion = model.Direccion;
            clienteDb.CiudadMunicipio = model.CiudadMunicipio;
            clienteDb.TipoCliente = model.TipoCliente;
            clienteDb.Observaciones = model.Observaciones;
            clienteDb.VIP = model.VIP;
            clienteDb.Activo = model.Activo;

            _context.Clientes.Update(clienteDb);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR CLIENTE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Activo = false;
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente eliminado (marcado como inactivo).";
            return RedirectToAction(nameof(Index));
        }

        // RESTAURAR CLIENTE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Activo = true;
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente restaurado.";
            return RedirectToAction(nameof(Index), new { soloEliminados = true });
        }
    }
}

// prueba git


/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioWEB.Data;
using InventarioWEB.Models;
using X.PagedList;

namespace InventarioWEB.Controllers
{
    public class ClientesController : Controller
    {
        private readonly MovimientoVentasDbContext _context;

        // ======================================================
        // Developer Note: Constructor del controlador
        // Recibe el contexto de base de datos de MovimientoVentas
        // ======================================================
        public ClientesController(MovimientoVentasDbContext context)
        {
            _context = context;
        }

        // ======================================================
        // Developer Note: LISTADO CON PAGINACIÓN Y FILTRO
        // Este método lista clientes activos o inactivos según el parámetro soloEliminados
        // Utiliza X.PagedList para la paginación
        // ======================================================
        public async Task<IActionResult> Index(int? page, bool soloEliminados = false)
        {
            int pageSize = 10; // Developer Note: Tamaño de página predeterminado
            int pageNumber = page ?? 1;

            IQueryable<Cliente> query = _context.Clientes;

            // Developer Note: Filtrado por clientes activos/inactivos
            query = soloEliminados
                ? query.Where(c => !c.Activo)
                : query.Where(c => c.Activo);

            var clientes = await query
                .OrderBy(c => c.Nombre) // Developer Note: Orden alfabético por nombre
                .ToPagedListAsync(pageNumber, pageSize);

            ViewBag.SoloEliminados = soloEliminados; // Developer Note: Se pasa a la vista para control de botones

            return View(clientes);
        }

        // ======================================================
        // Developer Note: DETALLES DEL CLIENTE
        // Muestra información completa de un cliente
        // ======================================================
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ID_Cliente == id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // ======================================================
        // Developer Note: CREAR CLIENTE (GET)
        // Inicializa un nuevo cliente con valores predeterminados
        // ======================================================
        public async Task<IActionResult> Create()
        {
            // Developer Note: Se carga lista de tipos de cliente desde base de datos
            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => t.Nombre)
                .ToListAsync();

            return View(new Cliente
            {
                FechaRegistro = DateTime.Now,
                VIP = true, // Developer Note: Por defecto, clientes nuevos son VIP true
                Activo = true
            });
        }

        // ======================================================
        // Developer Note: CREAR CLIENTE (POST)
        // Valida campos obligatorios, cédula y tipo de cliente
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente model)
        {
            ViewBag.Tipos = await _context.TipoCliente
                .Select(t => t.Nombre)
                .ToListAsync();

            // ------------------------------
            // Developer Note: VALIDACIÓN DE CÉDULA
            // Debe tener entre 6 y 10 dígitos, no comenzar con 0 y ser única
            // ------------------------------
            string idText = model.ID_Cliente.ToString();

            if (string.IsNullOrWhiteSpace(idText) || idText.Length < 6 || idText.Length > 10 || idText.StartsWith("0"))
            {
                ModelState.AddModelError(nameof(model.ID_Cliente),
                    "La cédula debe tener entre 6 y 10 dígitos y no puede empezar con 0.");
            }
            else if (await _context.Clientes.AnyAsync(c => c.ID_Cliente == model.ID_Cliente))
            {
                ModelState.AddModelError(nameof(model.ID_Cliente), "Ya existe un cliente con esa cédula.");
            }

            // ------------------------------
            // Developer Note: VALIDACIÓN DE CAMPOS OBLIGATORIOS
            // Observaciones no es obligatorio
            // ------------------------------
            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Apellido))
                ModelState.AddModelError(nameof(model.Apellido), "El apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Correo))
                ModelState.AddModelError(nameof(model.Correo), "El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Telefono))
                ModelState.AddModelError(nameof(model.Telefono), "El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Direccion))
                ModelState.AddModelError(nameof(model.Direccion), "La dirección es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.CiudadMunicipio))
                ModelState.AddModelError(nameof(model.CiudadMunicipio), "La ciudad o municipio es obligatoria.");

            if (!await _context.TipoCliente.AnyAsync(t => t.Nombre == model.TipoCliente))
                ModelState.AddModelError(nameof(model.TipoCliente), "Tipo de cliente inválido.");

            if (!ModelState.IsValid)
                return View(model);

            // ------------------------------
            // Developer Note: CAMPOS AUTOMÁTICOS
            // FechaRegistro, Activo y Hash de contraseña si no existe
            // ------------------------------
            model.FechaRegistro = DateTime.Now;
            model.Activo = true;

            if (string.IsNullOrWhiteSpace(model.HashContrasena))
            {
                model.Salt = Guid.NewGuid().ToString();
                model.HashContrasena = BCrypt.Net.BCrypt.HashPassword("default123" + model.Salt);
            }

            _context.Clientes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente guardado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // Developer Note: EDITAR CLIENTE (GET)
        // Carga datos existentes para edición
        // ======================================================
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            ViewBag.Tipos = await _context.TipoCliente.Select(t => t.Nombre).ToListAsync();

            return View(cliente);
        }

        // ======================================================
        // Developer Note: EDITAR CLIENTE (POST)
        // Valida campos obligatorios y actualiza registro en DB
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente model)
        {
            ViewBag.Tipos = await _context.TipoCliente.Select(t => t.Nombre).ToListAsync();

            if (id != model.ID_Cliente)
                return BadRequest();

            // Validaciones importantes
            if (string.IsNullOrWhiteSpace(model.Nombre))
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Apellido))
                ModelState.AddModelError(nameof(model.Apellido), "El apellido es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Correo))
                ModelState.AddModelError(nameof(model.Correo), "El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Telefono))
                ModelState.AddModelError(nameof(model.Telefono), "El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(model.Direccion))
                ModelState.AddModelError(nameof(model.Direccion), "La dirección es obligatoria.");
            if (string.IsNullOrWhiteSpace(model.CiudadMunicipio))
                ModelState.AddModelError(nameof(model.CiudadMunicipio), "La ciudad o municipio es obligatoria.");

            if (!await _context.TipoCliente.AnyAsync(t => t.Nombre == model.TipoCliente))
                ModelState.AddModelError(nameof(model.TipoCliente), "Tipo de cliente inválido.");

            if (!ModelState.IsValid)
                return View(model);

            var clienteDb = await _context.Clientes.FindAsync(id);
            if (clienteDb == null) return NotFound();

            // Developer Note: Actualizar campos de cliente en la DB
            clienteDb.Nombre = model.Nombre;
            clienteDb.Apellido = model.Apellido;
            clienteDb.Telefono = model.Telefono;
            clienteDb.Correo = model.Correo;
            clienteDb.Direccion = model.Direccion;
            clienteDb.CiudadMunicipio = model.CiudadMunicipio;
            clienteDb.TipoCliente = model.TipoCliente;
            clienteDb.Observaciones = model.Observaciones;
            clienteDb.VIP = model.VIP;
            clienteDb.Activo = model.Activo;

            _context.Clientes.Update(clienteDb);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // Developer Note: ELIMINAR CLIENTE
        // Marca como inactivo sin borrar de la base de datos
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Activo = false;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente eliminado (marcado como inactivo).";

            return RedirectToAction(nameof(Index));
        }

        // ======================================================
        // Developer Note: RESTAURAR CLIENTE
        // Reactiva un cliente previamente inactivo
        // ======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Activo = true;

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente restaurado.";

            return RedirectToAction(nameof(Index), new { soloEliminados = true });
        }
    }
}
*/