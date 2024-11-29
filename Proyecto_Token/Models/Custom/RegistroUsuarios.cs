using Microsoft.AspNetCore.Mvc; // Para ControllerBase y IActionResult
using Microsoft.EntityFrameworkCore; // Para acceder a DbContext y LINQ
using System.Linq; // Para usar métodos LINQ como .Any() o .ToList()
using System.Threading.Tasks; // Para async/await
using Proyecto_Token.Models.Custom; // Para el modelo RegistroUsuarios
namespace Proyecto_Token.Models.Custom;

public class RegistroUsuarios
{
    public string Nombre { get; set; }

    public string Correo { get; set; }

    public string Contraseña { get; set; }

    public int IdRol { get; set; }

}
