using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Feeds.Frontend.Services;
using Feeds.Shared.Data;
using Feeds.Shared.Enums;
using Feeds.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Feeds.Frontend.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IServicioUsuario _usuario;
        private readonly Cloudinary _cloudinary;

        public LoginController(IHttpClientFactory httpClientFactory, IServicioUsuario usuario, Cloudinary cloudinary)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7023/");
            _usuario = usuario;
            _cloudinary = cloudinary;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Login/Login", content);
                var user = await _usuario.GetUsuarioByEmail(model.CorreoElectronico);

                if (response.IsSuccessStatusCode)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.CorreoElectronico),
                        new Claim(ClaimTypes.Role, user.Rol.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    var servicioToken = new ServicioToken();
                    await servicioToken.Autenticar(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["AlertMessage"] = "Usuario o clave incorrectos!!!";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Registro()
        {
            Usuario usuario = new()
            {
                Rol = Rol.Autor
            };

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario, IFormFile? file)
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
                        return View(usuario);
                    }

                    var urlImagen = uploadResult.SecureUrl.ToString();
                    usuario.URLFoto = urlImagen;
                }

                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Login/Registro", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Usuario registrado exitosamente!!!";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error, no se pudo crear el usuario");
                }
            }
            return View(usuario);
        }
    }
}