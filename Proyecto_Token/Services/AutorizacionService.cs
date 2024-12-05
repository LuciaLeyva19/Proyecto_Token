using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Proyecto_Token.Models;
using Proyecto_Token.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


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

            var usuario = _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefault(u=>u.IdUsuario.ToString() == IdUsuario);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var key = _configuration.GetValue<string>("JwtSettings:key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, IdUsuario));
            claims.AddClaim(new Claim(ClaimTypes.Role, usuario.IdRolNavigation.NombreRol));

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

        private string GenerarRefreshToken() {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var mg = RandomNumberGenerator.Create()) { 
                mg.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }
            return refreshToken;
        }

        private async Task<AutorizacionResponse> GuardarHistorialRefreshToken(
            int idUsuario,
            string token,
            string refreshToken
            ) {
            var historialRefreshToken = new HistorialRefreshToken
            {
                IdUsuario = idUsuario,
                Token = token,
                RefreshToken = refreshToken,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(2)
            };

            await _context.HistorialRefreshTokens.AddAsync(historialRefreshToken);
            await _context.SaveChangesAsync();

            return new AutorizacionResponse { Token=token,RefreshToken=refreshToken, Resultado=true,Msg="Ok"};
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

            string refreshTokenCreado = GenerarRefreshToken();

            //return new AutorizacionResponse() { Token = tokenCreado, Resultado = true,Msg = "Okey" };

            return await GuardarHistorialRefreshToken(usuario_encotrado.IdUsuario,tokenCreado,refreshTokenCreado);


        }

        public async Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest refreshTokenRequest, int idUsuario)
        {
            var refreshTokenEncontrado = _context.HistorialRefreshTokens.FirstOrDefault(x =>
            x.Token == refreshTokenRequest.TokenExpirado &&
            x.RefreshToken == refreshTokenRequest.RefreshToken &&
            x.IdUsuario == idUsuario);

            if (refreshTokenEncontrado == null)
                return new AutorizacionResponse { Resultado = false, Msg = "No existe refresh Token" };

            var refreshTokenCreado = GenerarRefreshToken();
            var tokenCreado = GenerarToken(idUsuario.ToString());

            return await GuardarHistorialRefreshToken(idUsuario, tokenCreado, refreshTokenCreado);



        }

        public async Task<Usuario> RegistrarUsuarioAsync(RegistroUsuarios registroUsuario)
        {
            // Verificar si el correo ya existe
            if (_context.Usuarios.Any(u => u.Correo == registroUsuario.Correo))
            {
                throw new Exception("El correo ya está registrado.");
            }

            // Crear el nuevo usuario
            var nuevoUsuario = new Usuario
            {
                Nombre = registroUsuario.Nombre,
                Correo = registroUsuario.Correo,
                Contraseña= registroUsuario.Contraseña,
                IdRol = registroUsuario.IdRol
            };

            // Guardar en la base de datos
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return nuevoUsuario;
        }
        public async Task<Usuario> ObtenerUsuarioPorNombreAsync(string nombreUsuario)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);
        }

        public bool ValidarContraseña(Usuario usuario, string contraseña)
        {
            // Para este ejemplo, se usa una comparación simple de texto plano.
            // En un entorno real, utiliza un hash seguro para la contraseña.
            return usuario.Contraseña == contraseña;
        }
    }
 }

