﻿namespace Proyecto_Token.Models.Custom
{
    public class RefreshTokenRequest
    {
        public string TokenExpirado {  get; set; }
        public string RefreshToken { get; set; }
    }
}
