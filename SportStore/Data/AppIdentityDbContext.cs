using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration configuration;

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        
    }
    
}


