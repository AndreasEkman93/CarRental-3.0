using System.Threading.Tasks;
using CarRental.Models;
using CarRentalClient.Services;
using CarRentalClient.Services.Authentication;
using CarRentalClient.Services.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarRental
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();


            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();         
            
            builder.Services.AddHttpClient<IClient, Client>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7054");
            });
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            //builder.Services.AddAuthentication();
            //builder.Services.AddAuthentication("JwtCookie")
            //    .AddCookie("JwtCookie", options =>
            //    {
            //        options.Cookie.Name = "JwtCookie";
            //        options.LoginPath = "/Authentication/Login";
            //        options.LogoutPath = "/Authentication/Logout";
            //    });
            builder.Services.AddAuthorization();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();;

            app.Run();
        }
    }
}
