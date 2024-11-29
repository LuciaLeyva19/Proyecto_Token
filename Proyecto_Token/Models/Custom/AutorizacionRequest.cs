namespace Proyecto_Token.Models.Custom
{
    public class AutorizacionRequest
    {
        //Informacion utilizada al momento de recibir informacion del formulario de login
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
    }
}
