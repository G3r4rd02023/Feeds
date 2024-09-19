using Feeds.Frontend.Models;
using Feeds.Shared.Data;
using Newtonsoft.Json;
using System.Text;

namespace Feeds.Frontend.Services
{
    public class ServicioToken
    {
        public async Task<string> Autenticar(Usuario usuario)
        {
            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri("https://localhost:7023/");

            var credenciales = new Usuario()
            {
                Nombre = usuario.Nombre,
                Contrasena = usuario.Contrasena,
                CorreoElectronico = usuario.CorreoElectronico,
                FechaRegistro = usuario.FechaRegistro,
                URLFoto = usuario.URLFoto,
                Rol = usuario.Rol
            };

            var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync("api/Auth/Validar", content);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<Credencial>(json);
            var token = resultado!.Token;
            return token;
        }
    }
}