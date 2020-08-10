using BDMT.Client.Serverside;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BDMT.Client
{
    public static class StartupExtensions
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public static void AddUIBase(this IServiceCollection services)
        {
            services.AddScoped<IFormRedirectService, FormRedirectService>();
            services.AddScoped<IModeManager, ModeManager>();

            var currentAssembly = typeof(StartupExtensions).Assembly;
            services.AddFluxor(options => options.ScanAssemblies(currentAssembly).UseReduxDevTools());
        }
    }
}