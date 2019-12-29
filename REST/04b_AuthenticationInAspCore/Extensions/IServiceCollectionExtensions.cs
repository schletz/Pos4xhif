using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

#nullable enable
namespace AuthenticationDemo.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCookieAuthentication(this IServiceCollection services, bool setDefault)
        {
            services.AddCookieAuthentication(setDefault, TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Aktiviert die Cookie Based Authentication in ASP.NET Core.
        /// </summary>
        /// <param name="services"></param>
        public static void AddCookieAuthentication(this IServiceCollection services, bool setDefault,
            TimeSpan expiration)
        {
            services.AddAuthentication(options =>
            {
                if (setDefault)
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = expiration;
                options.LoginPath = "/";
                // Alternative: Statt dem Redirect zu options.LoginPath senden wir 401 Unauthorized.
                //options.Events.OnRedirectToLogin = context =>
                //{
                //    context.Response.StatusCode = 401;
                //    return Task.CompletedTask;
                //};
            });
        }
        /// <summary>
        /// Aktiviert die JWT Authentication in ASP.NET Core.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secret">Base64 codiertes Secret, welches für die Validierung
        /// des Tokens verwendet wird.</param>
        public static void AddJwtAuthentication(this IServiceCollection services,
            string secret,
            bool setDefault)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is null.", nameof(secret));
            }

            byte[] key = Convert.FromBase64String(secret);
            services.AddAuthentication(options =>
            {
                if (setDefault)
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }
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
                        {
                            ctx.Token = token;
                        }
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
        }
    }
}
