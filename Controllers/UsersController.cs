using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;
using StreamHubApi.Models;

namespace StreamHubApi.Controllers
{
    [ApiController]
    [Route("users")]
    [Tags("Usuarios")]
    public class UsersController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsersController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        [EndpointSummary("Registrar un nuevo usuario")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterDto dto)
        {
            var usuarioCreado = await _usuarioService.RegistrarAsync(dto);
            if (usuarioCreado == null)
                return BadRequest(new { mensaje = "Ya existe un usuario registrado con ese email." });

            return CreatedAtAction(nameof(GetById), new { id = usuarioCreado.Id }, usuarioCreado);
        }

        [HttpPost("login")]
        [EndpointSummary("Iniciar sesión y obtener token JWT")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
        {
            var auth = await _usuarioService.LoginAsync(dto);
            if (auth == null)
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });

            return Ok(auth);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpGet]
        [EndpointSummary("Listar todos los usuarios (solo Administrador)")]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [EndpointSummary("Consultar perfil de usuario por ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var tieneIdPropio = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int miId) && miId == id;
            if (!User.IsInRole(Roles.Administrador) && !tieneIdPropio)
                return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "No tenés permiso para ver este usuario." });

            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = $"No se encontró el usuario con ID {id}." });

            return Ok(usuario);
        }

        [Authorize(Roles = Roles.Administrador)]
        [HttpPut("{id:int}/role")]
        [EndpointSummary("Cambiar el rol de un usuario (solo Administrador)")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] UsuarioRolDto dto)
        {
            if (!Roles.Todos.Contains(dto.Rol))
                return BadRequest(new { mensaje = $"Rol inválido. Valores permitidos: {string.Join(", ", Roles.Todos)}." });

            // Evita quedarse sin administradores por error.
            var esMiUsuario = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int miId) && miId == id;
            if (esMiUsuario && dto.Rol != Roles.Administrador)
                return BadRequest(new { mensaje = "No podés quitarte a vos mismo el rol de Administrador." });

            var actualizado = await _usuarioService.CambiarRolAsync(id, dto.Rol);
            if (actualizado == null)
                return NotFound(new { mensaje = $"No se encontró el usuario con ID {id}." });

            return Ok(actualizado);
        }
    }
}