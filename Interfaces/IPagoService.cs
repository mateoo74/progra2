using StreamHubApi.Models;

namespace StreamHubApi.Interfaces
{
    public interface IPagoService
    {
        string GenerarCodigoPago(Suscripcion suscripcion, DateTime referencia);
        Task<Boleta> ObtenerOCrearBoletaActualAsync(Suscripcion suscripcion);
        Task<List<Boleta>> GenerarBoletasMesAsync();
        Task<object> ConciliarPagosAsync();
    }
}