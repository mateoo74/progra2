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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _context.PasarelasPago.ToListAsync();
            return Ok(lista);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PasarelaPago pasarela)
        {
            pasarela.UrlBase = pasarela.UrlBase.TrimEnd('/');
            _context.PasarelasPago.Add(pasarela);
            await _context.SaveChangesAsync();
            return Ok(pasarela);
        }
    }
}