using BDMT.Client.Clientside.Hosting;
using BDMT.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BDMT.Client.Clientside
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var baseAddress = builder.HostEnvironment.BaseAddress;
            builder.RootComponents.Add<App>("wasmapp");

            builder.Services.AddUIBase();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

            builder.Services.AddGrpcChannel(baseAddress);
            builder.Services.AddGrpcCodeFirstService<IWeatherService>();
            builder.Services.AddGrpcCodeFirstService<IModeInfoService>();
            builder.Services.AddGrpcCodeFirstService<IUserInfoService>();

            await builder.Build()
                .RunAsync();
        }
    }
}