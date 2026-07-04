using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Data;
using BankITDeskHelp.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
});

// Register services
builder.Services.AddScoped<BankITDeskHelp.Services.ITicketMetricsService, BankITDeskHelp.Services.TicketMetricsService>();

// Add MVC
builder.Services.AddControllersWithViews();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply pending migrations and seed data
try
{
    using (var scope = app.Services.CreateScope())
    {
        Console.WriteLine("Starting database migration...");
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during migration: {ex.Message}");
            throw;
        }

        // Seed roles
        try
        {
            Console.WriteLine("Starting role seeding...");
            await SeedData.SeedRolesAsync(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding roles: {ex.Message}");
        }

        // Seed master data
        try
        {
            Console.WriteLine("Starting master data seeding...");
            await SeedData.SeedMasterDataAsync(dbContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding master data: {ex.Message}");
        }

        // Seed users
        try
        {
            Console.WriteLine("Starting user seeding...");
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            await SeedData.SeedAdminUserAsync(userManager, configuration);
            await SeedData.SeedManagerUserAsync(userManager, configuration, dbContext);
            Console.WriteLine("User seeding completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding users: {ex.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Critical error during startup: {ex.Message}\n{ex.StackTrace}");
    // Don't throw - allow app to continue running
}

app.Run();
