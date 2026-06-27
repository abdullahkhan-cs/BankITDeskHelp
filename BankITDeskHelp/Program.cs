using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Data;
using BankITDeskHelp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register services
builder.Services.AddScoped<BankITDeskHelp.Services.ITicketMetricsService, BankITDeskHelp.Services.TicketMetricsService>();

// Add MVC
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedRolesAsync(scope.ServiceProvider);
}

using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedRolesAsync(scope.ServiceProvider);

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData.SeedMasterDataAsync(dbContext);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedData.SeedAdminUserAsync(userManager);
    await SeedData.SeedManagerUserAsync(userManager);
}

app.Run();
