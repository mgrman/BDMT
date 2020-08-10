using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BDMT.Client
{
    public interface IFormRedirectService
    {
        Task SubmitFormAsync(string endpoint, Dictionary<string, string>? values, HttpMethod method);
    }

    public class FormRedirectService : IFormRedirectService
    {
        private IJSRuntime jSRuntime;

        public FormRedirectService(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        public async Task SubmitFormAsync(string endpoint, Dictionary<string, string>? values, HttpMethod method)
        {
            await this.jSRuntime.InvokeVoidAsync("createAndSubmitForm", endpoint, method.Method, values ?? new Dictionary<string, string>());
        }
    }
}