using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            UserManager<IdentityUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //seeds an admin user
            IdentityUser? user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser("admin@example.com");
                user.Email = "admin@example.com";
                user.PhoneNumber = "555-1234";
                user.EmailConfirmed = true;
                
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
