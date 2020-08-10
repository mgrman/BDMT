using Microsoft.AspNetCore.Authorization;
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
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceGuardAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Validate(AuthorizeAttribute attr)
        {
            Validate(attr.Policy??"", attr.Roles ?? "", attr.AuthenticationSchemes ?? "");
        }

        public void Validate(string policy, string roles, string authenticationSchemes)
        {
            if (!(httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false))
            {
                throw new AuthenticationException();
            }
        }
    }
}
