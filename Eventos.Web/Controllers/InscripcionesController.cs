using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _repositorio;

    public InscripcionesController(InscripcionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Inscripciones
    public async Task<IActionResult> Index()
    {
        var inscripciones = await _repositorio.ObtenerTodasAsync();
        return View(inscripciones);
    }

    // GET: Inscripciones/Create
    public async Task<IActionResult> Create()
    {
        await CargarListasDesplegablesAsync();
        return View(new Inscripcion());
    }

    // POST: Inscripciones/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        // 1. Obtener el evento seleccionado
        var evento = await _repositorio.ObtenerEventoActivoAsync(inscripcion.EventoId);
        if (evento == null)
        {
            ModelState.AddModelError("EventoId", "El evento seleccionado no es válido o ya no se encuentra activo.");
        }
        else
        {
            // Validación 1: Que el evento no haya pasado
            if (evento.Fecha <= DateTime.Now)
            {
                ModelState.AddModelError("EventoId", "No se puede realizar la inscripción debido a que el evento ya ha pasado.");
            }

            // Validación 2: Que el aforo no esté completo (inscripciones Confirmada < Aforo)
            var totalConfirmadas = await _repositorio.ContarConfirmadasPorEventoAsync(evento.Id);
            if (totalConfirmadas >= evento.Aforo)
            {
                ModelState.AddModelError("EventoId", $"El aforo de este evento está completo ({totalConfirmadas}/{evento.Aforo} cupos ocupados).");
            }
        }

        // Validación 3: Que el asistente no esté ya inscrito (Confirmada) en ese evento
        if (inscripcion.EventoId > 0 && inscripcion.AsistenteId > 0)
        {
            var yaInscrito = await _repositorio.ExisteInscripcionConfirmadaAsync(inscripcion.EventoId, inscripcion.AsistenteId);
            if (yaInscrito)
            {
                ModelState.AddModelError("AsistenteId", "El asistente seleccionado ya cuenta con una inscripción 'Confirmada' en este evento.");
            }
        }

        if (ModelState.IsValid)
        {
            await _repositorio.CrearAsync(inscripcion);
            TempData["Mensaje"] = "¡La inscripción se ha registrado con estado 'Confirmada' exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasDesplegablesAsync(inscripcion.EventoId, inscripcion.AsistenteId);
        return View(inscripcion);
    }

    // POST: Inscripciones/Cancelar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cancelado = await _repositorio.CancelarAsync(id);
        if (cancelado)
        {
            TempData["Mensaje"] = "La inscripción ha sido cancelada exitosamente (Estado: Cancelada).";
        }
        else
        {
            TempData["Error"] = "No se pudo cancelar la inscripción solicitada.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasDesplegablesAsync(int? eventoIdSeleccionado = null, int? asistenteIdSeleccionado = null)
    {
        var eventos = await _repositorio.ObtenerEventosActivosParaInscripcionAsync();
        var asistentes = await _repositorio.ObtenerAsistentesActivosParaInscripcionAsync();

        ViewBag.EventoId = new SelectList(
            eventos.Select(e => new {
                e.Id,
                Descripcion = $"{e.Titulo} - {e.Fecha:dd/MM/yyyy HH:mm} ({e.Modalidad})"
            }),
            "Id",
            "Descripcion",
            eventoIdSeleccionado
        );

        ViewBag.AsistenteId = new SelectList(
            asistentes.Select(a => new {
                a.Id,
                Descripcion = $"{a.Nombres} (DNI: {a.Dni})"
            }),
            "Id",
            "Descripcion",
            asistenteIdSeleccionado
        );
    }
}
