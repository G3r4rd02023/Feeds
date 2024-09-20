using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Feeds.Frontend.Services
{
    public class ServicioLista : IServicioLista
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public ServicioLista(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/");
            _usuario = usuario;
        }

        public async Task<IEnumerable<SelectListItem>> GetListaCategorias(Usuario usuario)
        {
            var user = await _usuario.GetUsuarioByEmail(usuario.CorreoElectronico);
            var tokenService = new ServicioToken();
            var token = await tokenService.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Categorias");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content);

                var listaCategorias = categorias!.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Nombre
                }).ToList();

                listaCategorias.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Seleccione una categoria"
                });
                return listaCategorias;
            }
            return [];
        }
    }
}