using BDMT.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDMT.Server.Services
{
    public class ModeInfoService : IModeInfoService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IReadOnlyList<string> availableModes;

        public ModeInfoService(IHttpContextAccessor httpContextAccessor, IOptions<ModesConfiguration> modesConfiguration)
        {
            this.httpContextAccessor = httpContextAccessor;

            availableModes = modesConfiguration.Value.SupportedModes.Select(o => o.Name).ToArray();
        }

        public Task<IReadOnlyList<string>> GetAvailableModes() => Task.FromResult(availableModes);

        public Task<string> GetModeWhichShouldBeActive()
        {
            return Task.FromResult(GetModeWhichShouldBeActiveInner());
        }

        private string GetModeWhichShouldBeActiveInner()
        {
            var httpContext = this.httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("THis method can only be called on thread with active HttpContext!");
            }

            httpContext.Request.Cookies.TryGetValue("mode", out var activeMode);
            if (activeMode == null || !availableModes.Contains(activeMode))
            {
                activeMode = availableModes.First();
            }
            return activeMode;
        }
    }
}