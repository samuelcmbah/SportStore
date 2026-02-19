using Microsoft.AspNetCore.Builder;

namespace SportStore.Extensions
{
    public static class ApplicationRoutingExtensions
    {
        public static void MapApplicationRoutes(this WebApplication app)
        {
            // AREA ROUTES (must be first)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "catpage",
                pattern: "{category}/Page{productPage:int}",
                defaults: new { controller = "Home", action = "Index" }
            );

            app.MapControllerRoute(
                name: "page",
                pattern: "Page{productPage:int}",
                defaults: new { controller = "Home", action = "Index", productPage = 1 }
            );

            app.MapControllerRoute(
                name: "category",
                pattern: "{category}",
                defaults: new { controller = "Home", action = "Index", productPage = 1 }
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.MapDefaultControllerRoute();
        }
    }
}
