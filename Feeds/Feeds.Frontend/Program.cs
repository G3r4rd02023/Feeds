using CloudinaryDotNet;
using Feeds.Frontend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Feeds.Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IServicioUsuario, ServicioUsuario>();
            builder.Services.AddScoped<IServicioLista, ServicioLista>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Login/Login";
               options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
           });

            var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");

            var cloudinary = new Cloudinary(new Account(
             cloudinaryConfig["CloudName"],
             cloudinaryConfig["ApiKey"],
             cloudinaryConfig["ApiSecret"]
             ));

            builder.Services.AddSingleton(cloudinary);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Login}/{id?}");

            app.Run();
        }
    }
}