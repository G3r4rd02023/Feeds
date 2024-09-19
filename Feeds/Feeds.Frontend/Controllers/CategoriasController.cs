using Feeds.Frontend.Services;
using Feeds.Shared.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Feeds.Frontend.Controllers
{
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
            var response = await _httpClient.GetAsync("/api/Categorias");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<IEnumerable<Categoria>>(content);
                return View("Index", categorias);
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
            }
            return View(categoria);
        }

        public async Task<IActionResult> Edit(int id)
        {
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