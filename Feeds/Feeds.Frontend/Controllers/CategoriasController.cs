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
    public class CategoriasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;

        public CategoriasController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario)
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
            var response = await _httpClient.GetAsync("/api/Categorias");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content);
                return View("Index", categorias);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(new List<Categoria>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                var tokenService = new ServicioToken();
                var token = await tokenService.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(categoria);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Categorias", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Categoria creada exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al crear la categoria";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }
            }

            return View(categoria);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            var tokenService = new ServicioToken();
            var token = await tokenService.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/Categorias/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener categoria";
                return RedirectToAction("Index");
            }
            var jsonString = await response.Content.ReadAsStringAsync();
            var categoria = JsonConvert.DeserializeObject<Categoria>(jsonString);

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                var tokenService = new ServicioToken();
                var token = await tokenService.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(categoria);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Categorias/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Categoria actualizada exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al actualizar la categoria";
                    return RedirectToAction("Index");
                }
            }
            return View(categoria);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            var tokenService = new ServicioToken();
            var token = await tokenService.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/Categorias/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["AlertMessage"] = "Categoria eliminada Exitosamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Error al eliminar la categoria";
                return RedirectToAction("Index");
            }
        }
    }
}