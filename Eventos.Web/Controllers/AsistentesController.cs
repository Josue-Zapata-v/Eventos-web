using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _repositorio;

    public AsistentesController(AsistenteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // GET: Asistentes
    public async Task<IActionResult> Index()
    {
        var asistentes = await _repositorio.ObtenerActivosAsync();
        return View(asistentes);
    }

    // GET: Asistentes/Create
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    // POST: Asistentes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        // Validar unicidad de DNI
        if (!string.IsNullOrWhiteSpace(asistente.Dni))
        {
            var existeDni = await _repositorio.ExisteDniAsync(asistente.Dni);
            if (existeDni)
            {
                ModelState.AddModelError("Dni", "Ya existe un asistente registrado con este número de DNI.");
            }
        }

        // Validar unicidad de Correo
        if (!string.IsNullOrWhiteSpace(asistente.Correo))
        {
            var existeCorreo = await _repositorio.ExisteCorreoAsync(asistente.Correo);
            if (existeCorreo)
            {
                ModelState.AddModelError("Correo", "Ya existe un asistente registrado con este correo electrónico.");
            }
        }

        if (ModelState.IsValid)
        {
            await _repositorio.CrearAsync(asistente);
            TempData["Mensaje"] = "¡El asistente ha sido registrado correctamente!";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}
