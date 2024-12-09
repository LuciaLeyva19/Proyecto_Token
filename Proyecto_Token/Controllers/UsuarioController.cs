using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Token.Models.Custom;
using Proyecto_Token.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Proyecto_Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IAutorizacionService _autorizacionService;

        public UsuarioController(IAutorizacionService autorizacionService)
        {
            _autorizacionService = autorizacionService;
        }

        [HttpPost]
        [Route("Autenticar")]
        public async Task <IActionResult>Autenticar([FromBody] AutorizacionRequest autorizacion) 
        {
            // Validar si el usuario existe en la base de datos
            var usuario = await _autorizacionService.ObtenerUsuarioPorNombreAsync(autorizacion.NombreUsuario);
            if (usuario == null)
            {
                return Unauthorized(new { Mensaje = "El nombre de usuario no existe." });
            }

            // Valida si la contraseña es correcta
            var contraseñaValida = _autorizacionService.ValidarContraseña(usuario, autorizacion.Clave);
            if (!contraseñaValida)
            {
                return Unauthorized(new { Mensaje = "La contraseña es incorrecta." });
            }

            // Generar el token si las credenciales son correctas
            var resultado_autorizacion = await _autorizacionService.DevolverToken(autorizacion);
            if (resultado_autorizacion == null)
            {
                return Unauthorized(new { Mensaje = "Error al generar el token." });
            }

            return Ok(resultado_autorizacion);
        }


        [HttpPost]
        [Route("ObtenerRefreshToken")]
        public async Task<IActionResult> ObtenerRefreshToken([FromBody] RefreshTokenRequest request) { 

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExpiradoSupuestamente = tokenHandler.ReadJwtToken(request.TokenExpirado);

            if (tokenExpiradoSupuestamente.ValidTo > DateTime.UtcNow)
                return BadRequest(new AutorizacionResponse { Resultado = false, Msg = "Token no ha expirado" });

            string idUsuario = tokenExpiradoSupuestamente.Claims.First(x =>
            x.Type==JwtRegisteredClaimNames.NameId).Value.ToString();

            var autorizacionResponse = await _autorizacionService.DevolverRefreshToken(request, int.Parse(idUsuario));

            if (autorizacionResponse.Resultado)
                return Ok(autorizacionResponse);
            else
                return BadRequest(autorizacionResponse);
        }

        [HttpPost("Registro")]
        [Authorize]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarios registroUsuario)
        {
            try
            {
                // Obtener los claims del usuario autenticado
                var usuarioActualClaims = HttpContext.User.Claims;
                var rolUsuarioActual = usuarioActualClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Verificar si el usuario es administrador
                if (rolUsuarioActual != "Administrador")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "Solo los administradores pueden registrar nuevos usuarios" });
                }

                // Llamar al servicio para registrar al usuario
                var usuario = await _autorizacionService.RegistrarUsuarioAsync(registroUsuario);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
