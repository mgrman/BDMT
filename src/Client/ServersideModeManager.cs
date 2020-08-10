using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BDMT.Client.Serverside
{
    public class ModeManager : IModeManager
    {
        private readonly IFormRedirectService formRedirectService;
        private readonly NavigationManager navigationManager;

        public ModeManager(IFormRedirectService formRedirectService, NavigationManager navigationManager)
        {
            this.formRedirectService = formRedirectService;
            this.navigationManager = navigationManager;
        }

        public async Task SwitchModeAsync(string mode)
        {
            await this.formRedirectService.SubmitFormAsync("modes/active", new Dictionary<string, string> { { "mode", mode }, { "returnUrl", navigationManager.Uri } }, HttpMethod.Post);
        }
    }
}