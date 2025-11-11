
using System.Security.Claims;
using System.Text;
using CarRental.Data;
using CarRental.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarRentalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            { //Använde för att få  hashade passwords till existerande databasen.
                //var hasher = new PasswordHasher<ApplicationUser>();

                //var hashedpassword = hasher.HashPassword(null, "Qwe123!");
                //Console.WriteLine($"PW: {hashedpassword}");
            }
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            builder.Services.AddAuthentication( options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer( options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                    RoleClaimType = "role"
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;

                        // Map any long URI role claims to "role"
                        var longUriRoleClaim = identity.FindFirst(ClaimTypes.Role);
                        if (longUriRoleClaim != null)
                        {
                            identity.AddClaim(new Claim("role", longUriRoleClaim.Value));
                        }

                        return Task.CompletedTask;
                    }
                };
                //options.Events = new JwtBearerEvents
                //{
                //    OnTokenValidated = context =>
                //    {
                //        Console.WriteLine("Token validated. Claims:");
                //        foreach (var c in context.Principal.Claims)
                //        {
                //            Console.WriteLine($"{c.Type} = {c.Value}");
                //        }
                //        return Task.CompletedTask;
                //    },
                //    OnAuthenticationFailed = context =>
                //    {
                //        Console.WriteLine("Auth failed: " + context.Exception.Message);
                //        return Task.CompletedTask;
                //    },
                //    OnChallenge = context =>
                //    {
                //        Console.WriteLine("Unauthorized request: " + context.Error);
                //        return Task.CompletedTask;
                //    }
                //};
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();



            //builder.Services.AddScoped<IOrder, OrderRepository>();
            builder.Services.AddScoped<ICar, CarRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapControllers();

            app.Run();
        }
    }
}
