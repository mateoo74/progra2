using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.Interfaces;

namespace StreamHubApi.Controllers
{
    [ApiController]
    [Route("payments")]
    [Tags("Pagos y Boletas")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        private readonly StreamHubDbContext _context;

        public PaymentsController(IPagoService pagoService, StreamHubDbContext context)
        {
            _pagoService = pagoService;
            _context = context;
        }

        [Authorize]
        [HttpPost("invoices/generate")]
        [EndpointSummary("Genera las boletas del mes actual para las suscripciones existentes")]
        public async Task<IActionResult> GenerateCurrentInvoices()
        {
            var boletas = await _pagoService.GenerarBoletasMesAsync();
            return Ok(boletas);
        }

        [Authorize]
        [HttpGet("invoices")]
        [EndpointSummary("Lista las boletas generadas")]
        public async Task<IActionResult> GetInvoices()
        {
            var lista = await _context.Boletas.OrderByDescending(b => b.Id).ToListAsync();
            return Ok(lista);
        }

        [Authorize]
        [HttpPost("reconcile")]
        [EndpointSummary("Verifica pagos en PayLink y actualiza estados de suscripciones")]
        public async Task<IActionResult> Reconcile()
        {
            try
            {
                var resultado = await _pagoService.ConciliarPagosAsync();
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}