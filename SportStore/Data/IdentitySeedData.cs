using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data
{
    public static class IdentitySeedData
    {
        private const string adminRole = "Administrator";

        public static async Task EnsurePopulated(IApplicationBuilder app)
        {

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<AppIdentityDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var config = services.GetRequiredService<IConfiguration>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            var adminEmail = config["AdminCredentials:Email"];
            var adminPassword = config["AdminCredentials:Password"];


            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new InvalidOperationException(
                    "Admin credentials are not configured. Set AdminCredentials:Email and AdminCredentials:Password.");
            }

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            }

            //seeds an admin user
            var user = await userManager.FindByNameAsync(adminEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    PhoneNumber = "555-1234",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, adminPassword);
            }

            // Add the user to the admin role
            if (user != null && !await userManager.IsInRoleAsync(user, adminRole))
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }
}
