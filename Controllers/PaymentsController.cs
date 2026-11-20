using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

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

        [Authorize(Roles = Roles.Administrador)]
        [HttpPost("invoices/generate")]
        [EndpointSummary("Genera las boletas del mes actual para las suscripciones existentes")]
        public async Task<IActionResult> GenerateCurrentInvoices()
        {
            var boletas = await _pagoService.GenerarBoletasMesAsync();
            return Ok(boletas);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpGet("invoices")]
        [EndpointSummary("Lista las boletas generadas")]
        public async Task<IActionResult> GetInvoices()
        {
            var lista = await _context.Boletas.OrderByDescending(b => b.Id).ToListAsync();
            return Ok(lista);
        }

        [Authorize(Roles = Roles.Administrador)]
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