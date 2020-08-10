using BDMT.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BDMT.Server.Services
{
    public class ModeService : IModeService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IModeInfoService modeInfoService;
        private readonly ModesConfiguration modesConfiguration;

        public ModeService(IHttpContextAccessor httpContextAccessor, IOptions<ModesConfiguration> modesConfiguration, IModeInfoService modeInfoService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.modeInfoService = modeInfoService;
            this.modesConfiguration = modesConfiguration.Value;
        }

        public async Task<string> GetViewAsync()
        {
            var activeMode = await modeInfoService.GetModeWhichShouldBeActive();

            return modesConfiguration.SupportedModes.First(o => o.Name == activeMode).ViewPath;
        }

        public async Task SwitchModeAsync(string mode)
        {
            if (this.httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("THis method can only be called on thread with active HttpContext!");
            }

            var option = new CookieOptions
            {
                Expires = DateTime.MaxValue,
                HttpOnly = true,
            };

            this.httpContextAccessor.HttpContext.Response.Cookies.Append("mode", mode, option);
        }
    }
}