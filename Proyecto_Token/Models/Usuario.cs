using System;
using System.Collections.Generic;

namespace Proyecto_Token.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; }

    public string Correo { get; set; }

    public string Contraseña { get; set; }

    public int IdRol { get; set; }

    public virtual ICollection<HistorialRefreshToken> HistorialRefreshTokens { get; } = new List<HistorialRefreshToken>();

    public virtual Role IdRolNavigation { get; set; }
}
