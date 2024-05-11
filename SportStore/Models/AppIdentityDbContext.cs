using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportStore.Models
{
    public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration configuration;

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //creates a database folder if it doesnt already exists
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //if "bin" is present, remove all the path starting from "bin" word
            if (appDirectory.Contains("bin"))
            {
                int index = appDirectory.IndexOf("bin");
                appDirectory = appDirectory.Substring(0, index);
            }

            // Ensure the "database" directory exists
            var databaseDirectory = System.IO.Path.Combine(appDirectory, "database");
            if (!System.IO.Directory.Exists(databaseDirectory))
            {
                System.IO.Directory.CreateDirectory(databaseDirectory);
            }

            var DbPath = System.IO.Path.Join(databaseDirectory, "SportStoreIdentity.db");
            Console.WriteLine(DbPath);//debub line

            options.UseSqlite($"Data Source={DbPath}");
        }
    }
    
}


