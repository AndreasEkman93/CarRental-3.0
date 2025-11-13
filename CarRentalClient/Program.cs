using System.Threading.Tasks;
using CarRental.Data;
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
            builder.Services.AddHttpClient<ICarService, CarService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7054");
            });
            builder.Services.AddHttpClient<IOrderService, OrderService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7054");
            });

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddAuthentication();
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
