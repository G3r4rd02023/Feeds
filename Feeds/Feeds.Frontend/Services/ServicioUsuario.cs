using Feeds.Shared.Data;
using Newtonsoft.Json;

namespace Feeds.Frontend.Services
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly HttpClient _httpClient;

        public ServicioUsuario(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/");
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            return usuario!;
        }
    }
}