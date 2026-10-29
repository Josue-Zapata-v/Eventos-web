using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.ViewModels;

public class EventoItemViewModel
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
    public int Aforo { get; set; }
    public decimal Precio { get; set; }
    public int InscritosConfirmados { get; set; }
    public int CuposDisponibles => Math.Max(0, Aforo - InscritosConfirmados);
}

public class EventoIndexViewModel
{
    public List<EventoItemViewModel> Eventos { get; set; } = new();
    public SelectList? Categorias { get; set; }
    public int? CategoriaId { get; set; }
}
