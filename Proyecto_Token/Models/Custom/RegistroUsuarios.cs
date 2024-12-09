using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 
using System.Linq; 
using System.Threading.Tasks; 
using Proyecto_Token.Models.Custom; 
namespace Proyecto_Token.Models.Custom;

public class RegistroUsuarios
{
    public string Nombre { get; set; }

    public string Correo { get; set; }

    public string Contraseña { get; set; }

    public int IdRol { get; set; }

}
