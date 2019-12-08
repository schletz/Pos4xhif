using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AuthExample.App.Model;
using AuthExample.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthExample.App.Extensions
{
    public static class HostingExtrensions
    {
        /// <summary>
        /// Erlaubt, von allen URLs aus auf die REST API zuzugreifen.
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });
        }

        /// <summary>
        /// Setzt den DbContext auf die übergebene SQLite Datenbank. Durch Dependency Injection wird in
        /// den Controllerklassen im Konstruktor der Context automatisch übergeben.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="database"></param>
        public static void ConfigureDatabase(this IServiceCollection services, string database)
        {
            services.AddDbContext<SchuleContext>(options =>
                options.UseSqlite($"DataSource={database}")
            );
        }

        /// <summary>
        /// Setzt die Einstellungen für die JWT Authentifizierung und setzt den UserService für
        /// die Dependency Injection im Konstruktor.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secret"></param>
        public static void ConfigureJwt(this IServiceCollection services, string secret)
        {
            // Vgl. https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api

            byte[] key = Convert.FromBase64String(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Damit der Token auch als GET Parameter in der Form ...?token=xxxx übergben
                // werden kann, reagieren wir auf den Event für ankommende Anfragen.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        string token = ctx.Request.Query["token"];
                        if (!string.IsNullOrEmpty(token))
                            ctx.Token = ctx.Request.Query["token"];
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Dependency Injection für das Userservice im Konstruktor von UserController
            services.AddScoped<UserService>();
        }
    }
}
