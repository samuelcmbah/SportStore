using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Data
{
    public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration configuration;

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

       
    }
    
}


