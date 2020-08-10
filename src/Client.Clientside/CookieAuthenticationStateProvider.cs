using BDMT.Client.Clientside.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BDMT.Client.Clientside
{
    public class CookieAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly ClaimsPrincipal AnonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly IUserInfoService userInfoService;
        private readonly IJSInProcessRuntime? runtime;
        private Task<AuthenticationState> _cachedAuthenticationState;

        public CookieAuthenticationStateProvider(IUserInfoService userInfoService, IJSRuntime runtime)
        {
            this.userInfoService = userInfoService;
            this.runtime = runtime as IJSInProcessRuntime;

            if (this.runtime == null)
            {
                _cachedAuthenticationState = GetAuthenticationStateFromServerAsync();
            }
            else
            {
                try
                {
                    var httpContextUser = this.runtime.Invoke<UserInfo>("getHttpContextUser");
                    if (httpContextUser != null)
                    {
                        _cachedAuthenticationState = Task.FromResult(GetAuthenticationStateFromServer(httpContextUser));
                    }
                    else
                    {
                        _cachedAuthenticationState = GetAuthenticationStateFromServerAsync();
                    }
                }
                catch
                {
                    _cachedAuthenticationState = GetAuthenticationStateFromServerAsync();
                }
            }
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return _cachedAuthenticationState;
        }

        protected async Task<AuthenticationState> GetAuthenticationStateFromServerAsync()
        {
            var userInfo = await this.userInfoService.GetAsync();
            return GetAuthenticationStateFromServer(userInfo);
        }

        protected AuthenticationState GetAuthenticationStateFromServer(UserInfo userInfo)
        {
            ClaimsPrincipal user;
            if (userInfo.IsAuthenticated)
            {
                user = new ClaimsPrincipal(new ClaimsIdentity(userInfo.Claims.Select(o => new Claim(o.Type, o.Value)), "cookie"));
            }
            else
            {
                user = AnonymousUser;
            }

            return new AuthenticationState(user);
        }
    }
}