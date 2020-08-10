using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using BDMT.Server;

namespace BDMT.Client.Serverside
{
    public static class StartupExtensions
    {
        public static void UseServersideUI(this IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public static void MapServersideUI(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapBlazorHub();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void AddServersideUI(this IServiceCollection services)
        {
            services.AddUIBase();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.PostConfigure<ModesConfiguration>(myOptions =>
            {
                myOptions.SupportedModes.Add(new ModeConfig("Server-side", "Serverside"));
            });

            services.AddControllersWithViews()
                .AddApplicationPart(typeof(StartupExtensions).Assembly);
        }
    }
}