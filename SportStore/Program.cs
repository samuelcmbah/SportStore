using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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
using System;


public class Program
{
    public static void Main(string[] args)
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
        }).AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("SportsStoreConnection")));

        builder.Services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("SportsStoreConnection")));

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

        app.MapApplicationRoutes();

        SeedData.EnsurePopulated(app);
        IdentitySeedData.EnsurePopulated(app);

        app.UseSerilogRequestLogging();

        app.Run();
    }
}
