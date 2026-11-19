using StreamHubApi.DTOs;

namespace StreamHubApi.Interfaces
{
    public interface ISuscripcionService
    {
        Task<SuscripcionResponseDto?> CrearSuscripcionAsync(SuscripcionCreateDto dto);
        Task<IEnumerable<SuscripcionResponseDto>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<SuscripcionResponseDto?> ActualizarEstadoAsync(int suscripcionId, string nuevoEstado);
        Task<SuscripcionResponseDto?> CancelarSuscripcionUsuarioAsync(int usuarioId);
    }
}