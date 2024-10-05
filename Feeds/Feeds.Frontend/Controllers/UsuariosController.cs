using Feeds.Frontend.Services;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            var tokenService = new ServicioToken();
            var token = await tokenService.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/Usuarios/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener usuario";
                return RedirectToAction("Index");
            }
            var jsonString = await response.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(jsonString);

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                var tokenService = new ServicioToken();
                var token = await tokenService.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Usuarios/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Usuario actualizado exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al actualizar el usuario";
                    return RedirectToAction("Index");
                }
            }
            return View(usuario);
        }
    }
}