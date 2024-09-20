using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Feeds.Frontend.Models;
using Feeds.Frontend.Services;
using Feeds.Shared.Data;
using Feeds.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace Feeds.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;
        private readonly IServicioLista _lista;
        private readonly Cloudinary _cloudinary;

        public HomeController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario, IServicioLista lista, Cloudinary cloudinary)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/");
            _usuario = usuario;
            _lista = lista;
            _cloudinary = cloudinary;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Posts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var posts = JsonConvert.DeserializeObject<IEnumerable<Entrada>>(content);
                return View("Index", posts!.OrderByDescending(p => p.FechaPublicacion));
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(new List<Entrada>());
        }

        public async Task<IActionResult> Create()
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var categorias = await _lista.GetListaCategorias(user);

            var model = new PublicacionViewModel()
            {
                FechaPublicacion = DateTime.Now,
                UsuarioId = user.Id,
                Estado = Estado.Borrador,
                Categorias = categorias,
                Usuario = user,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PublicacionViewModel model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        AssetFolder = "tecnologers"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error al cargar la imagen.");
                        return View(model.Usuario);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    model.URLImagen = urlImagen;
                }

                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                var tokenService = new ServicioToken();
                var token = await tokenService.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                model.UsuarioId = user.Id;
                model.Usuario = user;
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Posts", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Tu publicacion fue realizada exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al crear la publicacion";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Login");
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}