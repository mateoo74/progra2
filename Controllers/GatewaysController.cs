using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamHubApi.Context;
using StreamHubApi.Models;

namespace StreamHubApi.Controllers
{
    [ApiController]
    [Route("gateways")]
    [Tags("Pasarela de Pagos")]
    public class GatewaysController : ControllerBase
    {
        private readonly StreamHubDbContext _context;

        public GatewaysController(StreamHubDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _context.PasarelasPago.ToListAsync();
            return Ok(lista);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PasarelaPago pasarela)
        {
            pasarela.UrlBase = pasarela.UrlBase.TrimEnd('/');
            _context.PasarelasPago.Add(pasarela);
            await _context.SaveChangesAsync();
            return Ok(pasarela);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pasarela = await _context.PasarelasPago.FindAsync(id);
            if (pasarela == null)
                return NotFound(new { mensaje = $"No se encontró la pasarela con ID {id}." });

            _context.PasarelasPago.Remove(pasarela);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}