using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Services
{
    public class PagoService : IPagoService
    {
        private readonly StreamHubDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PagoService(StreamHubDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public string GenerarCodigoPago(Suscripcion suscripcion, DateTime referencia)
        {
            var periodo = referencia.ToString("yyyyMM");
            return $"SUB-{suscripcion.Id}-{periodo}";
        }

        public async Task<Boleta> ObtenerOCrearBoletaActualAsync(Suscripcion suscripcion)
        {
            var ahora = DateTime.UtcNow;
            var periodo = ahora.ToString("yyyyMM");
            var codigo = GenerarCodigoPago(suscripcion, ahora);

            var existente = await _context.Boletas
                .FirstOrDefaultAsync(b => b.SuscripcionId == suscripcion.Id && b.Periodo == periodo);

            if (existente != null)
                return existente;

            var boleta = new Boleta
            {
                SuscripcionId = suscripcion.Id,
                CodigoPago = codigo,
                Periodo = periodo,
                FechaVencimiento = suscripcion.FechaInicioPeriodo.AddDays(30),
                Estado = "Pendiente"
            };

            _context.Boletas.Add(boleta);
            await _context.SaveChangesAsync();
            return boleta;
        }

        public async Task<List<Boleta>> GenerarBoletasMesAsync()
        {
            var suscripciones = await _context.Suscripciones.ToListAsync();
            var resultado = new List<Boleta>();

            foreach (var sub in suscripciones)
            {
                var boleta = await ObtenerOCrearBoletaActualAsync(sub);
                resultado.Add(boleta);
            }

            return resultado;
        }

        public async Task<object> ConciliarPagosAsync()
        {
            var pasarela = await _context.PasarelasPago.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
            if (pasarela == null)
                throw new InvalidOperationException("No hay ninguna PasarelaPago configurada en la base de datos.");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Remove("X-Api-Key");
            client.DefaultRequestHeaders.Add("X-Api-Key", pasarela.ApiKey);

            var baseUrl = pasarela.UrlBase.TrimEnd('/');
            var suscripciones = await _context.Suscripciones.ToListAsync();
            var ahora = DateTime.UtcNow;

            int aprobadas = 0;
            int pendientesDePago = 0;
            var detalles = new List<object>();

            foreach (var sub in suscripciones)
            {
                var boleta = await ObtenerOCrearBoletaActualAsync(sub);
                var url = $"{baseUrl}/api/payments?invoiceId={Uri.EscapeDataString(boleta.CodigoPago)}";

                List<PayLinkPaymentItem>? pagos = null;
                try
                {
                    pagos = await client.GetFromJsonAsync<List<PayLinkPaymentItem>>(url);
                }
                catch (Exception ex)
                {
                    detalles.Add(new
                    {
                        suscripcionId = sub.Id,
                        codigoPago = boleta.CodigoPago,
                        errorConsulta = ex.Message
                    });
                    continue;
                }

                bool estaAprobado = pagos != null &&
                    pagos.Any(p => string.Equals(p.State, "APPROVED", StringComparison.OrdinalIgnoreCase));

                if (estaAprobado)
                {
                    boleta.Estado = "Pagada";
                    sub.Estado = "Activa";
                    aprobadas++;
                }
                else if (ahora > boleta.FechaVencimiento)
                {
                    if (!string.Equals(sub.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
                    {
                        sub.Estado = "Pendiente de Pago";
                        pendientesDePago++;
                    }
                }

                detalles.Add(new
                {
                    suscripcionId = sub.Id,
                    codigoPago = boleta.CodigoPago,
                    estadoBoleta = boleta.Estado,
                    estadoSuscripcion = sub.Estado,
                    aprobadoEnPasarela = estaAprobado
                });
            }

            await _context.SaveChangesAsync();

            return new
            {
                totalEvaluadas = suscripciones.Count,
                activasPorPago = aprobadas,
                marcadasPendienteDePago = pendientesDePago,
                detalle = detalles
            };
        }

        private sealed class PayLinkPaymentItem
        {
            public string? State { get; set; }
        }
    }
}