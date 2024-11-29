using Proyecto_Token.Models.Custom;
namespace Proyecto_Token.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion);
    }
}
