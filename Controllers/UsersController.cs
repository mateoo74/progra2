using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamHubApi.DTOs;
using StreamHubApi.Interfaces;

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

        [Authorize]
        [HttpGet("{id:int}")]
        [EndpointSummary("Consultar perfil de usuario por ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { mensaje = $"No se encontró el usuario con ID {id}." });

            return Ok(usuario);
        }
    }
}