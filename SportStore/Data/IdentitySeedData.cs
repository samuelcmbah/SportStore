using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportStore.Models;

namespace SportStore.Data
{
    public static class IdentitySeedData
    {
        private const string adminUser = "admin@example.com";
        private const string adminPassword = "Secret123$";
        private const string adminRole = "Administrator";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            //Gets the required DbContext services
            AppIdentityDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<AppIdentityDbContext>();

           

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Get the UserManager and RoleManager services
            UserManager<ApplicationUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //seeds an admin user
            ApplicationUser? user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    PhoneNumber = "555-1234",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, adminPassword);
            }

            // Check if the admin role exists and create it if it doesn't
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Add the user to the admin role
            if (user != null && !await userManager.IsInRoleAsync(user, adminRole))
            {
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }
}
