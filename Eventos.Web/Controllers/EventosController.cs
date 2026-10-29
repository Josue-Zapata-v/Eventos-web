using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Eventos.Web.ViewModels;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _repositorio;

    public EventosController(EventoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Eventos
    public async Task<IActionResult> Index(int? categoriaId)
    {
        var eventos = await _repositorio.ObtenerActivosAsync(categoriaId);
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();

        var viewModel = new EventoIndexViewModel
        {
            CategoriaId = categoriaId,
            Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId),
            Eventos = eventos.Select(e => new EventoItemViewModel
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Modalidad = e.Modalidad,
                CategoriaNombre = e.Categoria?.Nombre ?? "Sin categoría",
                Aforo = e.Aforo,
                Precio = e.Precio,
                InscritosConfirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            }).ToList()
        };

        return View(viewModel);
    }

    // GET: Eventos/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);
        if (evento == null)
        {
            return NotFound();
        }

        return View(evento);
    }

    // GET: Eventos/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasDesplegablesAsync();
        var model = new Evento
        {
            Fecha = DateTime.Today.AddDays(7).AddHours(9),
            Aforo = 50,
            Precio = 0.00m,
            Modalidad = "Presencial"
        };
        return View(model);
    }

    // POST: Eventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (evento.Fecha <= DateTime.Now)
        {
            ModelState.AddModelError("Fecha", "La fecha del evento debe ser posterior a la fecha actual.");
        }

        if (ModelState.IsValid)
        {
            await _repositorio.CrearAsync(evento);
            TempData["Mensaje"] = "¡El evento se ha registrado correctamente!";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // GET: Eventos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var evento = await _repositorio.ObtenerParaEdicionAsync(id.Value);
        if (evento == null)
        {
            return NotFound();
        }

        await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // POST: Eventos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _repositorio.ActualizarAsync(evento);
            TempData["Mensaje"] = "¡El evento se ha actualizado correctamente!";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegablesAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    // GET: Eventos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var evento = await _repositorio.ObtenerPorIdAsync(id.Value);
        if (evento == null)
        {
            return NotFound();
        }

        return View(evento);
    }

    // POST: Eventos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _repositorio.EliminarLogicoAsync(id);
        if (eliminado)
        {
            TempData["Mensaje"] = "El evento ha sido dado de baja exitosamente.";
        }
        else
        {
            TempData["Error"] = "No se pudo eliminar el evento solicitado.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasDesplegablesAsync(int? categoriaSeleccionadaId = null, string? modalidadSeleccionada = null)
    {
        var categorias = await _repositorio.ObtenerCategoriasActivasAsync();
        ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionadaId);

        var modalidades = new List<SelectListItem>
        {
            new SelectListItem { Value = "Presencial", Text = "Presencial" },
            new SelectListItem { Value = "Virtual", Text = "Virtual" }
        };
        ViewBag.Modalidades = new SelectList(modalidades, "Value", "Text", modalidadSeleccionada ?? "Presencial");
    }
}
