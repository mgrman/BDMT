using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BDMT.Client.Clientside.Hosting;
using BDMT.Client.Serverside;
using BDMT.Server.Data;
using BDMT.Server.Models;
using BDMT.Server.Services;
using Microsoft.AspNetCore.StaticFiles;
using System;
using Npgsql;
using BDMT.Client;
using BDMT.Shared;
using ProtoBuf.Grpc.Server;
using ServiceGuard;

namespace BDMT.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = TryPostgresEnvironmentaVariable("DATABASE_URL")
                ?? TryConnectionStringsConfig("ApplicationDbContext");

            if (connectionString == null)
            {
                throw new InvalidOperationException("NoDBContextConfigured!");
            }

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddClientsideUI();
            services.AddServersideUI();

            services.AddOptions<ModesConfiguration>();
            services.AddScoped<IModeService, ModeService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ServiceGuard.IAuthenticationService, ServiceGuardAuthenticationService>();
            services.AddScoped<IModeInfoService, ModeInfoService_Guarded>();
            services.AddScoped<IWeatherService, WeatherService_Guarded>();
            services.AddScoped<IUserInfoService, UserInfoService_Guarded>();

            services.AddCodeFirstGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            var extensionProvider = new FileExtensionContentTypeProvider();
            extensionProvider.Mappings.Add(".gltf", "model/gltf+json");
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = extensionProvider
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseClientsideUI(env);
            app.UseServersideUI(env);

            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<WeatherService>()
                    .EnableGrpcWeb();
                endpoints.MapGrpcService<ModeInfoService>()
                    .EnableGrpcWeb();
                endpoints.MapGrpcService<UserInfoService>()
                    .EnableGrpcWeb();

                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapClientsideUI();
                endpoints.MapServersideUI();
                endpoints.MapFallbackToController("Show", "Mode");
            });
        }

        private string? TryPostgresEnvironmentaVariable(string variableName)
        {
            var databaseUrl = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(databaseUrl))
            {
                return null;
            }
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };
            return builder.ToString();
        }

        private string? TryConnectionStringsConfig(string name )
        {
            var connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>(name);
            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            connectionString = Environment.GetEnvironmentVariable(connectionString) ?? connectionString;
            return connectionString;
        }
    }
}