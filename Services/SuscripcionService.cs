using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Services
{
    public class SuscripcionService : ISuscripcionService
    {
        private readonly StreamHubDbContext _context;

        public SuscripcionService(StreamHubDbContext context)
        {
            _context = context;
        }

        public async Task<SuscripcionResponseDto?> CrearSuscripcionAsync(SuscripcionCreateDto dto)
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste) return null;

            var estadosValidos = new[] { "Activa", "Cancelada", "Pendiente de Pago" };
            var estadoInicial = !string.IsNullOrWhiteSpace(dto.Estado) && estadosValidos.Contains(dto.Estado)
                ? dto.Estado
                : "Pendiente de Pago";

            var nueva = new Suscripcion
            {
                UsuarioId = dto.UsuarioId,
                TipoPlan = dto.TipoPlan,
                FechaInicioPeriodo = DateTime.UtcNow,
                Estado = estadoInicial
            };

            _context.Suscripciones.Add(nueva);
            await _context.SaveChangesAsync();

            return MapearDto(nueva);
        }

        public async Task<IEnumerable<SuscripcionResponseDto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var lista = await _context.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaInicioPeriodo)
                .ToListAsync();

            return lista.Select(MapearDto);
        }

        public async Task<SuscripcionResponseDto?> ActualizarEstadoAsync(int suscripcionId, string nuevoEstado)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(suscripcionId);
            if (suscripcion == null) return null;

            suscripcion.Estado = nuevoEstado;
            if (nuevoEstado.Equals("Activa", StringComparison.OrdinalIgnoreCase))
            {
                suscripcion.FechaInicioPeriodo = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return MapearDto(suscripcion);
        }

        public async Task<SuscripcionResponseDto?> CancelarSuscripcionUsuarioAsync(int usuarioId)
        {
            var suscripcion = await _context.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaInicioPeriodo)
                .FirstOrDefaultAsync();

            if (suscripcion == null) return null;

            // Cambiamos el estado a "Cancelada" sin borrar FechaInicioPeriodo para respetar el mes ya abonado
            suscripcion.Estado = "Cancelada";
            await _context.SaveChangesAsync();

            return MapearDto(suscripcion);
        }

        private static SuscripcionResponseDto MapearDto(Suscripcion s) => new()
        {
            Id = s.Id,
            UsuarioId = s.UsuarioId,
            TipoPlan = s.TipoPlan,
            FechaInicioPeriodo = s.FechaInicioPeriodo,
            Estado = s.Estado
        };
    }
}