using System.Text.Json.Serialization;
using AutoMapper;
using Koop.Extensions;
using Koop.Mapper;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.Repositories;
using Koop.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Koop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfiles());
            });

            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    opts.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddDbContext<KoopDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<KoopDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IGenericUnitOfWork, GenericUnitOfWork>();

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            

            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();

            services.AddAuth(jwtSettings);
            services.AddIdentityPasswordPolicy();
            
            services.AddSwaggerExt();

            services.AddControllers().AddNewtonsoftJson(o =>
                o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerExt();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();
            
            app.UseAuth();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}