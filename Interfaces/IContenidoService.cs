using StreamHubApi.DTOs;

namespace StreamHubApi.Interfaces
{
    public interface IContenidoService
    {
        Task<IEnumerable<ContenidoCatalogoDto>> ObtenerTodosAsync();
        Task<ContenidoCatalogoDto?> ObtenerPorIdAsync(int id);
        Task<ContenidoCatalogoDto> CrearAsync(ContenidoCreateUpdateDto dto);
        Task<ContenidoCatalogoDto?> ActualizarAsync(int id, ContenidoCreateUpdateDto dto);
        Task<bool> EliminarAsync(int id);

        // Punto 3: Devuelve (ContenidoExiste, TieneAcceso, DatosReproduccion)
        Task<(bool Existe, bool Autorizado, PlayContentResponseDto? Datos)> VerificarYReproducirAsync(int contentId, int usuarioId);
    }
}