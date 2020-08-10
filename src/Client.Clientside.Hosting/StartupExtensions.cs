using BDMT.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BDMT.Client.Clientside.Hosting
{
    public static class StartupExtensions
    {
        public static void UseClientsideUI(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseBlazorFrameworkFiles();
        }

        public static void MapClientsideUI(this IEndpointRouteBuilder endpoints)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void AddClientsideUI(this IServiceCollection services)
        {
            services.PostConfigure<ModesConfiguration>(myOptions =>
            {
                myOptions.SupportedModes.Add(new ModeConfig("Client-side", "Clientside"));
                myOptions.SupportedModes.Add(new ModeConfig("Client-side (prerendered)", "ClientsideWithPrerender"));
            });
        }
    }
}