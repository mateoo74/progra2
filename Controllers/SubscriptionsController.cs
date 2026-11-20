using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Controllers
{
    [ApiController]
    [Route("subscriptions")]
    [Tags("Suscripciones")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISuscripcionService _suscripcionService;

        public SubscriptionsController(ISuscripcionService suscripcionService)
        {
            _suscripcionService = suscripcionService;
        }

        [Authorize]
        [HttpPost]
        [EndpointSummary("Asignar una suscripción a un usuario")]
        public async Task<IActionResult> Create([FromBody] SuscripcionCreateDto dto)
        {
            // Un usuario común solo puede suscribirse a sí mismo y no puede elegir el estado inicial.
            if (!User.IsInRole(Roles.Administrador))
            {
                if (!TryGetUserId(out int miId))
                    return Unauthorized(new { mensaje = "Token inválido." });

                dto.UsuarioId = miId;
                dto.Estado = null;
            }

            var creada = await _suscripcionService.CrearSuscripcionAsync(dto);
            if (creada == null)
                return NotFound(new { mensaje = $"El usuario con ID {dto.UsuarioId} no existe." });

            return Ok(creada);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpGet]
        [EndpointSummary("Listar todas las suscripciones (solo Administrador)")]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _suscripcionService.ObtenerTodasAsync();
            return Ok(lista);
        }

        [Authorize]
        [HttpGet("user/{usuarioId:int}")]
        [EndpointSummary("Consultar suscripciones de un usuario")]
        public async Task<IActionResult> GetByUser(int usuarioId)
        {
            if (!User.IsInRole(Roles.Administrador) && !(TryGetUserId(out int miId) && miId == usuarioId))
                return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "No tenés permiso para ver las suscripciones de otro usuario." });

            var lista = await _suscripcionService.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(lista);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpPatch("{id:int}/status")]
        [EndpointSummary("Actualizar estado de una suscripción (solo Administrador)")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SuscripcionEstadoDto dto)
        {
            var estadosValidos = new[] { "Activa", "Cancelada", "Pendiente de Pago" };
            if (!estadosValidos.Contains(dto.Estado))
                return BadRequest(new { mensaje = "Estado inválido. Valores permitidos: 'Activa', 'Cancelada', 'Pendiente de Pago'." });

            var actualizada = await _suscripcionService.ActualizarEstadoAsync(id, dto.Estado);
            if (actualizada == null)
                return NotFound(new { mensaje = $"No se encontró la suscripción con ID {id}." });

            return Ok(actualizada);
        }

        private bool TryGetUserId(out int usuarioId) =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out usuarioId);

        [Authorize]
        [HttpPost("cancel")]
        [EndpointSummary("Cancelar suscripción manteniendo acceso hasta fin de mes")]
        public async Task<IActionResult> CancelMySubscription()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int usuarioId))
                return Unauthorized(new { mensaje = "Token inválido." });

            var cancelada = await _suscripcionService.CancelarSuscripcionUsuarioAsync(usuarioId);
            if (cancelada == null)
                return NotFound(new { mensaje = "No tenés ninguna suscripción registrada." });

            return Ok(new
            {
                mensaje = "Suscripción cancelada. Mantendrás el acceso hasta completar los 30 días desde el inicio del período.",
                suscripcion = cancelada
            });
        }
    }
}