using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BDMT.Client.Clientside.Hosting
{

    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<UserInfo> GetAsync()
        {
            return Task.FromResult(Get());
        }

        public UserInfo Get()
        {
            var user = httpContextAccessor.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return new UserInfo
                {
                    IsAuthenticated = false,
                    Claims= Array.Empty<UserClaim>()
                };
            }

            var claims = user.Claims.Select(o => new UserClaim { Type= o.Type,Value= o.Value }).ToList();
            var info = new UserInfo
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                Claims = claims
            }; 
            return info;
        }
    }
}