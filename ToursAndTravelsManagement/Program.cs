using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog;
using ToursAndTravelsManagement.Data;
using ToursAndTravelsManagement.Middlewares;
using ToursAndTravelsManagement.Models;
using ToursAndTravelsManagement.Repositories;
using ToursAndTravelsManagement.Repositories.IRepositories;
using ToursAndTravelsManagement.Services;

namespace ToursAndTravelsManagement;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddTransient<IEmailService, EmailService>();

        // Configure authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
        });

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog(); // Add Serilog

        builder.Services.AddScoped<DataSeeder>();

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

        app.UseAuthentication();
        app.UseAuthorization();

        // Use the global exception handling middleware
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Use the global error handling middleware
        app.UseMiddleware<GlobalErrorHandlingMiddleware>();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Ensure the database is seeded with fake data
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dataSeeder = services.GetRequiredService<DataSeeder>();

            // Seed 100 fake destinations
            // dataSeeder.SeedDestinationsAsync(1000);

            // Seed 1000 fake tours
            //dataSeeder.SeedToursAsync(1000);

            // Seed 1000 fake bookings
            //dataSeeder.SeedBookingsAsync(1000);
        }

        app.Run();
    }
}
