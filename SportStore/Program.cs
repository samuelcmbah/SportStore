using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SportStore.Models;

namespace SportStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
            builder.Services.AddScoped<SessionCart>(serviceProvider =>
            {
                // Retrieve the HttpContextAccessor service
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

                // Retrieve the HttpContext from the HttpContextAccessor
                var httpContext = httpContextAccessor.HttpContext;

                // Retrieve the ISession service from the HttpContext
                var session = httpContext?.Session;

                // Create and return a new instance of SessionCart
                return new SessionCart(session);
            });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //builder.Services.AddHttpContextAccessor();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();//The AddScoped method creates a service where each HTTP request gets its own repository object, which is the way that Entity Framework Core is typically used.
            builder.Services.AddDbContext<StoreDbContext>(opts => {
                opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();


            app.MapControllerRoute(name: "catpage",
             pattern: "{category}/Page{productPage:int}",
             defaults: new { Controller = "Home", action = "Index" });

            app.MapControllerRoute(name: "page",
            pattern: "Page{productPage:int}",
            defaults: new { Controller = "Home", action = "Index", productPage = 1 });

            app.MapControllerRoute(name: "category",
            pattern: "{category}",
            defaults: new { Controller = "Home", action = "Index", productPage = 1 });

            //app.MapControllerRoute(name: "pagination",
            //    pattern: "Products/Page{productPage:int}",
            //     defaults: new { Controller = "Home", action = "Index", id = 1 });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapDefaultControllerRoute();

            SeedData.EnsurePopulated(app);

            app.Run();
        }
    }
}
