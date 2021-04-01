using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Koop.Models;
using Koop.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Koop.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services
                .AddAuthorization(o =>
                {
                    o.AddPolicy("Admin", p => p.RequireRole("Admin"));
                    o.AddPolicy("KoTy", p => p.RequireRole("Koty"));
                    o.AddPolicy("OpRo", p => p.RequireRole("OpRo"));
                    o.AddPolicy("Paczkers", p => p.RequireRole("Paczkers"));
                    o.AddPolicy("Wprowadzacz", p => p.RequireRole("Wprowadzacz"));
                    o.AddPolicy("Skarbnik", p => p.RequireRole("Skarbnik"));
                    o.AddPolicy("StandardUser", p => p.RequireRole("Default"));
                    o.AddPolicy("Szymek", p => p.RequireUserName("Szymek33"));
                })
                .AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}