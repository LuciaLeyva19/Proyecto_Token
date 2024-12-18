﻿using Proyecto_Token.Models;
using Proyecto_Token.Models.Custom;
namespace Proyecto_Token.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion);
        Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest refreshTokenRequest, int idUsuario);

        Task<Usuario> RegistrarUsuarioAsync(RegistroUsuarios registroUsuario);

        Task<Usuario> ObtenerUsuarioPorNombreAsync(string nombreUsuario);
        bool ValidarContraseña(Usuario usuario, string contraseña);

    }
}
