using Feeds.Frontend.Services;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Feeds.Frontend.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public UsuariosController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/");
            _usuario = usuario;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            var tokenService = new ServicioToken();
            var token = await tokenService.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/Usuarios");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
                return View("Index", usuarios);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(new List<Usuario>());
        }
    }
}