# Eventos.Web - ASP.NET Core MVC & Entity Framework Core Code First

Proyecto web desarrollado para la gestión de eventos, asistentes e inscripciones utilizando **ASP.NET Core MVC** y **Entity Framework Core (Code First)** con SQL Server.

## Características Principales

- **Entity Framework Core Code First**: Creación de base de datos (`EventosDB`) y tablas mediante migraciones sucesivas, sin scripts manuales ni procedimientos almacenados.
- **Configuración mediante Data Annotations**:
  - Índices únicos (`[Index(..., IsUnique = true)]`) para el nombre de categorías, DNI y correo de asistentes.
  - Columnas de precisión decimal (`[Column(TypeName = "decimal(10,2)")]`) para el precio de los eventos.
  - Validaciones de longitud, rangos de aforo y formato de correo electrónico.
- **Eliminación y Cancelación Lógica**:
  - Las entidades `Categoria`, `Evento` y `Asistente` manejan eliminación lógica mediante la columna `Activo` (`false`), preservando la integridad referencial.
  - La entidad `Inscripcion` maneja cancelación mediante el estado `"Cancelada"`.
- **Capa de Repositorios**:
  - Implementación del patrón repositorio (`EventoRepositorio`, `AsistenteRepositorio`, `InscripcionRepositorio`) inyectados mediante Dependency Injection con ciclo de vida `Scoped`.
  - Consultas LINQ asíncronas optimizadas con `AsNoTracking()` para lecturas.
- **Reglas de Negocio en Inscripciones**:
  - Validación de fecha: no permite inscribirse a eventos pasados.
  - Validación de duplicidad: un asistente solo puede tener una inscripción activa ("Confirmada") por evento.
  - Validación de aforo: control estricto de cupos disponibles en tiempo real.
- **Seed Data Automático**:
  - Carga inicial desde `Program.cs` de 3 categorías, 4 eventos y 5 asistentes si las tablas se encuentran vacías.

## Historial de Migraciones

El proyecto cuenta con exactamente tres migraciones aplicadas en orden:
1. `20261029011912_InitialCreate`: Creación inicial de las 4 entidades base (`Categorias`, `Eventos`, `Asistentes`, `Inscripciones`).
2. `20261029012112_AgregarModalidadEvento`: Incorporación de la propiedad `Modalidad` en `Evento` con valor por defecto `"Presencial"`.
3. `20261029012300_AgregarTelefonoAsistente`: Incorporación de la propiedad opcional `Telefono` en `Asistente`.

## Ejecución del Proyecto

```powershell
# Restaurar dependencias y compilar
dotnet build

# Iniciar la aplicación
cd Eventos.Web
dotnet run
```
