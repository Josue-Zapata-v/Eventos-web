using Microsoft.EntityFrameworkCore;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de Repositorios (Acceso a datos desacoplado)
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Inicialización de datos (Seed Data)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    // 1. Cargar 3 Categorías si está vacía
    if (!await context.Categorias.AnyAsync())
    {
        var categorias = new List<Categoria>
        {
            new Categoria { Nombre = "Tecnología", Activo = true },
            new Categoria { Nombre = "Negocios e Innovación", Activo = true },
            new Categoria { Nombre = "Arte y Diseño", Activo = true }
        };
        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();
    }

    // 2. Cargar 5 Asistentes si está vacía
    if (!await context.Asistentes.AnyAsync())
    {
        var asistentes = new List<Asistente>
        {
            new Asistente
            {
                Nombres = "Carlos Eduardo Mendoza",
                Dni = "45871234",
                Correo = "carlos.mendoza@gmail.com",
                Telefono = "987112233",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Ana Lucía Fernández",
                Dni = "71239845",
                Correo = "ana.fernandez@outlook.com",
                Telefono = "994556677",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Jorge Luis Paredes",
                Dni = "09874512",
                Correo = "jorge.paredes@hotmail.com",
                Telefono = "912334455",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Valeria Sofía Quispe",
                Dni = "63524190",
                Correo = "valeria.quispe@yahoo.com",
                Telefono = "976889900",
                Activo = true
            },
            new Asistente
            {
                Nombres = "Diego Alonso Morales",
                Dni = "84756123",
                Correo = "diego.morales@empresa.com",
                Telefono = "955223344",
                Activo = true
            }
        };
        await context.Asistentes.AddRangeAsync(asistentes);
        await context.SaveChangesAsync();
    }

    // 3. Cargar 4 Eventos si está vacía
    if (!await context.Eventos.AnyAsync())
    {
        var catTec = await context.Categorias.FirstOrDefaultAsync(c => c.Nombre == "Tecnología");
        var catNeg = await context.Categorias.FirstOrDefaultAsync(c => c.Nombre == "Negocios e Innovación");
        var catArte = await context.Categorias.FirstOrDefaultAsync(c => c.Nombre == "Arte y Diseño");

        var eventos = new List<Evento>
        {
            new Evento
            {
                Titulo = "Cumbre Latinoamericana de Inteligencia Artificial 2026",
                Fecha = DateTime.Today.AddDays(20).AddHours(9),
                Lugar = "Centro de Convenciones de Lima",
                Aforo = 350,
                Precio = 120.00m,
                Modalidad = "Presencial",
                CategoriaId = catTec!.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Masterclass: Microservicios y Cloud con .NET 10",
                Fecha = DateTime.Today.AddDays(10).AddHours(19),
                Lugar = "Transmisión Virtual Teams / Zoom",
                Aforo = 100,
                Precio = 50.00m,
                Modalidad = "Virtual",
                CategoriaId = catTec!.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Congreso Internacional de Startups y Venture Capital",
                Fecha = DateTime.Today.AddDays(35).AddHours(14),
                Lugar = "Auditorio Hotel Westin",
                Aforo = 200,
                Precio = 250.00m,
                Modalidad = "Presencial",
                CategoriaId = catNeg!.Id,
                Activo = true
            },
            new Evento
            {
                Titulo = "Exposición de Arte Digital y Diseño Generativo",
                Fecha = DateTime.Today.AddDays(15).AddHours(17),
                Lugar = "Galería Contemporánea Miraflores",
                Aforo = 80,
                Precio = 35.00m,
                Modalidad = "Presencial",
                CategoriaId = catArte!.Id,
                Activo = true
            }
        };
        await context.Eventos.AddRangeAsync(eventos);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
