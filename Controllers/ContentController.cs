using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;

namespace StreamHubApi.Controllers
{
    [ApiController]
    [Route("content")]
    [Tags("Contenidos")]
    public class ContentController : ControllerBase
    {
        private readonly IContenidoService _contenidoService;

        public ContentController(IContenidoService contenidoService)
        {
            _contenidoService = contenidoService;
        }

        // GET /content - Listado general del catálogo (sin exponer UrlStream)
        [HttpGet]
        [EndpointSummary("Listar catálogo completo de contenidos")]
        public async Task<IActionResult> GetAll()
        {
            var catalogo = await _contenidoService.ObtenerTodosAsync();
            return Ok(catalogo);
        }

        // GET /content/{id} - Detalle de un contenido específico
        [HttpGet("{id:int}")]
        [EndpointSummary("Obtener detalle de un contenido por ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _contenidoService.ObtenerPorIdAsync(id);
            if (item == null)
                return NotFound(new { mensaje = $"No se encontró contenido con ID {id}." });

            return Ok(item);
        }

        // POST /content - Crear nuevo contenido (requiere estar autenticado)
        [Authorize]
        [HttpPost]
        [EndpointSummary("Publicar nuevo contenido en el catálogo")]
        public async Task<IActionResult> Create([FromBody] ContenidoCreateUpdateDto dto)
        {
            var creado = await _contenidoService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // PUT /content/{id} - Actualizar contenido existente
        [Authorize]
        [HttpPut("{id:int}")]
        [EndpointSummary("Actualizar un contenido existente")]
        public async Task<IActionResult> Update(int id, [FromBody] ContenidoCreateUpdateDto dto)
        {
            var actualizado = await _contenidoService.ActualizarAsync(id, dto);
            if (actualizado == null)
                return NotFound(new { mensaje = $"No se encontró contenido con ID {id}." });

            return Ok(actualizado);
        }

        // DELETE /content/{id} - Eliminar contenido
        [Authorize]
        [HttpDelete("{id:int}")]
        [EndpointSummary("Eliminar un contenido del catálogo")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _contenidoService.EliminarAsync(id);
            if (!eliminado)
                return NotFound(new { mensaje = $"No se encontró contenido con ID {id}." });

            return NoContent();
        }

        // GET /content/{content_id}/play - Punto 3: Control de acceso y reproducción
        [Authorize]
        [HttpGet("{content_id:int}/play")]
        [EndpointSummary("Verificar suscripción y reproducir contenido")]
        public async Task<IActionResult> PlayContent(int content_id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int usuarioId))
            {
                return Unauthorized(new { mensaje = "Token inválido o sin identificador de usuario." });
            }

            var (existe, autorizado, datos) = await _contenidoService.VerificarYReproducirAsync(content_id, usuarioId);

            if (!existe)
                return NotFound(new { mensaje = $"El contenido con ID {content_id} no existe." });

            if (!autorizado)
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    mensaje = "Acceso denegado. Tu suscripción está Pendiente de Pago, Cancelada fuera de período o inexistente."
                });

            return Ok(datos);
        }
    }
}