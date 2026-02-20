
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resend;
using Serilog;
using SportStore.Configurations;
using SportStore.Data;
using SportStore.Extensions;
using SportStore.Middleware;
using SportStore.Models;
using SportStore.Services;
using SportStore.Services.IServices;
using SportStore.Utils;
using System.Globalization;

public class Program
{
    // Plan (pseudocode):
    // 1. Make Main asynchronous so we can properly await asynchronous seed methods and RunAsync.
    // 2. Build the WebApplication as before.
    // 3. After building and configuring middleware/routes, await IdentitySeedData.EnsurePopulated(app).
    // 4. Replace app.Run() with await app.RunAsync() so the async Main can await the host run.
    // 5. Keep all other code unchanged to avoid side effects.
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var payBridgeUrl = builder.Configuration["ExternalServices:PayBridgeUrl"];

        builder.Services.AddHttpClient("PayBridge", client =>
        {
            client.BaseAddress = new Uri(payBridgeUrl!);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

        });
        // Replace default logging with Serilog
        builder.Host.UseSerilog((_, logger) =>
        {
            logger
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console();
        });
        //adding email service
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.Configure<ResendEmailSettings>(
            builder.Configuration.GetSection("ResendEmailSettings"));
        // This is required to use the IOptions<T> pattern
        builder.Services.AddOptions();

        builder.Services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = builder.Configuration["ResendEmailSettings:ApiKey"]!;
        });

        // 5. Add the HttpClient and the main Resend client to the services
        builder.Services.AddHttpClient<ResendClient>();
        builder.Services.AddTransient<IResend, ResendClient>();
        //adding identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;

            // Configure lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

        }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("SportsStoreConnection")));

        builder.Services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("SportsStoreConnection")));

        // Configure Cloudinary settings
        builder.Services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("Cloudinary"));

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
        builder.Services.AddScoped<IOrderDomainService, OrderDomainService>();
        builder.Services.AddScoped<ICartDomainService, CartDomainService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
        builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<SessionCart>();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });


        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        var cultureInfo = new System.Globalization.CultureInfo("en-NG");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

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

        app.UseMiddleware<RoleBasedRootRedirectMiddleware>();

        app.MapApplicationRoutes();

        SeedData.EnsurePopulated(app);
        // Await the asynchronous seeding method to avoid CS4014.
        await IdentitySeedData.EnsurePopulated(app);

        app.UseSerilogRequestLogging();



        await app.RunAsync();
    }
}

