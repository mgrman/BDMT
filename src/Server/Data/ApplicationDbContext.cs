using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BDMT.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BDMT.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}