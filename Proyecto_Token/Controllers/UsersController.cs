using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Proyecto_Token.Models;

namespace Proyecto_Token.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GestionUsuariosContext _context;

        public UsersController(GestionUsuariosContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        [Route("Lista")]

        public async Task<IActionResult> Lista()
        {
            // Recuperar la lista de usuarios desde la base de datos
            var listaUsuarios = await _context.Usuarios
                                              .Select(u => new
                                              {
                                                  u.IdUsuario,
                                                  u.Nombre,
                                                  u.Correo,
                                                  u.IdRolNavigation
                                              })
                                              .ToListAsync();
           
            return Ok(listaUsuarios);
            
        }

    }
}
