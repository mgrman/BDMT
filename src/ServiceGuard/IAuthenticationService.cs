using Microsoft.AspNetCore.Authorization;

namespace ServiceGuard
{
    public interface IAuthenticationService
    {
        void Validate(AuthorizeAttribute attr);

        void Validate(string policy, string roles, string authenticationSchemes);
    }
}