using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Services
{
    public class ContenidoService : IContenidoService
    {
        private readonly StreamHubDbContext _context;

        public ContenidoService(StreamHubDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContenidoCatalogoDto>> ObtenerTodosAsync()
        {
            return await _context.Contenidos
                .Select(c => new ContenidoCatalogoDto
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    Tipo = c.Tipo,
                    Genero = c.Genero,
                    ClasificacionEdad = c.ClasificacionEdad
                })
                .ToListAsync();
        }

                public async Task<IEnumerable<ContenidoAdminDto>> ObtenerTodosAdminAsync()
        {
            return await _context.Contenidos
                .OrderBy(c => c.Id)
                .Select(c => new ContenidoAdminDto
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion,
                    Tipo = c.Tipo,
                    Genero = c.Genero,
                    ClasificacionEdad = c.ClasificacionEdad,
                    UrlStream = c.UrlStream
                })
                .ToListAsync();
        }

        public async Task<ContenidoCatalogoDto?> ObtenerPorIdAsync(int id)
        {
            var contenido = await _context.Contenidos.FindAsync(id);
            if (contenido == null) return null;

            return new ContenidoCatalogoDto
            {
                Id = contenido.Id,
                Titulo = contenido.Titulo,
                Descripcion = contenido.Descripcion,
                Tipo = contenido.Tipo,
                Genero = contenido.Genero,
                ClasificacionEdad = contenido.ClasificacionEdad
            };
        }

        public async Task<ContenidoCatalogoDto> CrearAsync(ContenidoCreateUpdateDto dto)
        {
            var nuevo = new Contenido
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Tipo = dto.Tipo,
                Genero = dto.Genero,
                ClasificacionEdad = dto.ClasificacionEdad,
                UrlStream = dto.UrlStream
            };

            _context.Contenidos.Add(nuevo);
            await _context.SaveChangesAsync();

            return new ContenidoCatalogoDto
            {
                Id = nuevo.Id,
                Titulo = nuevo.Titulo,
                Descripcion = nuevo.Descripcion,
                Tipo = nuevo.Tipo,
                Genero = nuevo.Genero,
                ClasificacionEdad = nuevo.ClasificacionEdad
            };
        }

        public async Task<ContenidoCatalogoDto?> ActualizarAsync(int id, ContenidoCreateUpdateDto dto)
        {
            var existente = await _context.Contenidos.FindAsync(id);
            if (existente == null) return null;

            existente.Titulo = dto.Titulo;
            existente.Descripcion = dto.Descripcion;
            existente.Tipo = dto.Tipo;
            existente.Genero = dto.Genero;
            existente.ClasificacionEdad = dto.ClasificacionEdad;
            existente.UrlStream = dto.UrlStream;

            await _context.SaveChangesAsync();

            return new ContenidoCatalogoDto
            {
                Id = existente.Id,
                Titulo = existente.Titulo,
                Descripcion = existente.Descripcion,
                Tipo = existente.Tipo,
                Genero = existente.Genero,
                ClasificacionEdad = existente.ClasificacionEdad
            };
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var existente = await _context.Contenidos.FindAsync(id);
            if (existente == null) return false;

            _context.Contenidos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Existe, bool Autorizado, PlayContentResponseDto? Datos)> VerificarYReproducirAsync(int contentId, int usuarioId)
        {
            var contenido = await _context.Contenidos.FindAsync(contentId);
            if (contenido == null)
            {
                return (false, false, null);
            }

            //se busca la suscripcion mas reciente del usuario
            var suscripcion = await _context.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaInicioPeriodo)
                .FirstOrDefaultAsync();

            if (suscripcion == null)
            {
                return (true, false, null);
            }

            // Acceso permitido si está "Activa", o si fue "Cancelada" pero sigue dentro de los 30 días del período pagado
            bool periodoVigente = suscripcion.FechaInicioPeriodo.AddMonths(1) >= DateTime.UtcNow;
            bool tieneAcceso = suscripcion.Estado.Equals("Activa", StringComparison.OrdinalIgnoreCase)
                || (suscripcion.Estado.Equals("Cancelada", StringComparison.OrdinalIgnoreCase) && periodoVigente);

            if (!tieneAcceso)
            {
                return (true, false, null);
            }

            var datos = new PlayContentResponseDto
            {
                ContentId = contenido.Id,
                Titulo = contenido.Titulo,
                UrlStream = contenido.UrlStream,
                Mensaje = "Suscripción vigente. Disfrutá del contenido."
            };

            return (true, true, datos);
        }
    }
}