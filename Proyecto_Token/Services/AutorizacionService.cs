using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Proyecto_Token.Models;
using Proyecto_Token.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Token.Services
{
    public class AutorizacionService : IAutorizacionService
    {
        private readonly GestionUsuariosContext _context;
        private readonly IConfiguration _configuration;

        public AutorizacionService(GestionUsuariosContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //metodo para generar token

        private string GenerarToken(string IdUsuario) {

            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, IdUsuario));

            //credencial para el token

            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //claims contiene la informacion de idusuarios
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = credencialesToken
            };

            //Controladores de jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return tokenCreado;
        }

        public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion)
        {
            var usuario_encotrado = _context.Usuarios.FirstOrDefault(x =>
            x.Nombre == autorizacion.NombreUsuario &&
            x.Contraseña == autorizacion.Clave
             

            );

            //Validacion
            if (usuario_encotrado == null) { 
                return await Task.FromResult<AutorizacionResponse>(null);
            }
            //Si existe genera token y devuelve el token por el mismo medio
            string tokenCreado = GenerarToken(usuario_encotrado.IdUsuario.ToString());

            return new AutorizacionResponse() { Token = tokenCreado, Resultado = true,Msg = "Okey" };


        }
    }
}
