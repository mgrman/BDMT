using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceGuard
{
    public interface IAuthenticationService
    {

        void Validate(AuthorizeAttribute attr);

        void Validate(string policy, string roles, string authenticationSchemes);
    }

}