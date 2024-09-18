using Feeds.Shared.Data;

namespace Feeds.Frontend.Services
{
    public interface IServicioUsuario
    {
        Task<Usuario> GetUsuarioByEmail(string email);
    }
}