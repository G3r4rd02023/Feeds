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

        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1)
        {
            var response = await _httpClient.GetAsync("/api/Posts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var posts = JsonConvert.DeserializeObject<IEnumerable<Entrada>>(content);

                // Filtrado
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    posts = posts!.Where(p => p.Titulo.Contains(searchTerm) || p.Contenido.Contains(searchTerm));
                }

                // Ordenar por fecha de publicaci�n descendente
                posts = posts!.OrderByDescending(p => p.FechaPublicacion);

                // Paginaci�n
                int pageSize = 3; // Puedes cambiar el tama�o de p�gina aqu�
                var paginatedPosts = PaginatedList<Entrada>.Create(posts.AsQueryable(), pageNumber, pageSize);

                return View("Index", paginatedPosts);
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
                    TempData["SuccessMessage"] = "Tu publicacion fue realizada exitosamente";
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

        public async Task<IActionResult> AgregarComentario(int postId)
        {
            var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var comentario = new Comentario()
            {
                UsuarioId = user.Id,
                EntradaId = postId,
                Usuario = user,
            };

            return View(comentario);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarComentario(Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUsuarioByEmail(User.Identity!.Name!);
                if (user == null)
                {
                    return NotFound();
                }

                var posts = await _httpClient.GetAsync("/api/Posts");
                if (posts.IsSuccessStatusCode)
                {
                    comentario.Usuario = user;
                    var postsContent = await posts.Content.ReadAsStringAsync();
                    var entrada = JsonConvert.DeserializeObject<IEnumerable<Entrada>>(postsContent);
                    var mypost = entrada!.FirstOrDefault(p => p.Id == comentario.EntradaId);
                    mypost!.Comentarios.Add(comentario);
                    var jsonComentario = JsonConvert.SerializeObject(mypost);
                    var contentComentario = new StringContent(jsonComentario, Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync("/api/Posts", contentComentario);
                }

                var json = JsonConvert.SerializeObject(comentario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Comentarios", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tu comentario se agrego exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al agregar el comentario";
                }
            }
            return View(comentario);
        }

        public async Task<IActionResult> MeGusta(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Posts/{id}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<Entrada>(jsonString);

            post!.Likes += 1;
            var json = JsonConvert.SerializeObject(post);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var postResponse = await _httpClient.PutAsync($"/api/Posts/{id}", content);
            if (postResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Te ha gustado esta publicacion";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Ocurrio un error inesperado";
                return Redirect("Index");
            }
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