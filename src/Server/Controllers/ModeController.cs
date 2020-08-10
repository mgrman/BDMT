using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BDMT.Client;

namespace BDMT.Server.Controllers
{
    [Controller]
    public class ModeController : Controller
    {
        private readonly ILogger<ModeController> logger;
        private readonly IModeService modeService;

        public ModeController(ILogger<ModeController> logger, IModeService modeService)
        {
            this.logger = logger;
            this.modeService = modeService;
        }

        [HttpPost]
        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> ShowAsync()
        {
            var viewName=await modeService.GetViewAsync();
            return View(viewName);
        }

        [HttpPost]
        [HttpPut]
        [Route("modes/active")]
        public async Task<IActionResult> SetActiveAsync([FromForm] string mode, [FromForm] string? returnUrl)
        {
            this.modeService.SwitchModeAsync(mode);
            return this.Redirect(returnUrl ?? "~/");
        }
    }
}