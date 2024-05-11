using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SportStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog;
using SportStore.Services;

namespace SportStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //configuring third party logging
            Log.Logger = new LoggerConfiguration().MinimumLevel.Error().WriteTo.File("Logs/SportStoreLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
            builder.Host.UseSerilog();
            //adding email service
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<EmailService>();
            //adding identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

            builder.Services.AddDbContext<AppIdentityDbContext>(opts =>
            {
                opts.UseSqlite(builder.Configuration["ConnectionStrings:IdentityConnection"]);
            });

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
                opts.UseSqlite(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
            });

            //builder.Services.AddDbContext<StoreDbContext>(opts => {
            //    opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
            //});

            builder.Services.AddControllersWithViews();
            //builder.Services.AddControllersWithViews(options =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //                     .RequireAuthenticatedUser()
            //                     .Build();
            //    options.Filters.Add(new AuthorizeFilter(policy));
            //});
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

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
            IdentitySeedData.EnsurePopulated(app);

            app.Run();
        }
    }
}
