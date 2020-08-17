using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace BDMT.Server.Services
{
    public class ServiceGuardAuthenticationService : ServiceGuard.IAuthenticationService
    {
        private readonly IAuthorizationPolicyProvider authorizationPolicyProvider;
        private readonly IAuthorizationService authorizationService;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public ServiceGuardAuthenticationService(IAuthorizationPolicyProvider authorizationPolicyProvider, IAuthorizationService authorizationService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.authorizationPolicyProvider = authorizationPolicyProvider;
            this.authorizationService = authorizationService;
            this.authenticationStateProvider = authenticationStateProvider;

        }

        public void Validate(string? policy, string? roles, string? authenticationSchemes)
        {
            var authorizeData = new AuthorizeDataAdapter(policy, roles, authenticationSchemes);

            var combinedPolicy = AuthorizationPolicy.CombineAsync(authorizationPolicyProvider, new[] { authorizeData }).Result;
            if (combinedPolicy == null)
            {
                throw new InvalidOperationException($"Could not combine policies!");
            }

            var authenticationState = authenticationStateProvider.GetAuthenticationStateAsync().Result;
            var user = authenticationState.User;

            var result = authorizationService.AuthorizeAsync(user, null, combinedPolicy).Result;
            if (!result.Succeeded)
            {
                throw new AuthenticationException();
            }
        }

        internal class AuthorizeDataAdapter : IAuthorizeData
        {
            public AuthorizeDataAdapter(string? policy, string? roles, string? authenticationSchemes)
            {
                this.Policy = policy;
                this.Roles = roles;
                this.AuthenticationSchemes = authenticationSchemes;
            }

            public string? Policy
            {
                get;
                set;
            }

            public string? Roles
            {
                get;
                set;
            }

            public string? AuthenticationSchemes
            {
                get;
                set;
            }
        }
    }
}
