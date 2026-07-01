using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Models;

namespace BankITDeskHelp.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedMasterDataAsync(ApplicationDbContext context)
        {
            if (!await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department { Name = "ATM Support" },
                    new Department { Name = "Core Banking" },
                    new Department { Name = "Internet Banking" },
                    new Department { Name = "Network & Infrastructure" },
                    new Department { Name = "Software Support" }
                );
            }

            if (!await context.Branches.AnyAsync())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Main Branch", Location = "Karachi" },
                    new Branch { Name = "Nawabshah Branch", Location = "Nawabshah" },
                    new Branch { Name = "Hyderabad Branch", Location = "Hyderabad" }
                );
            }

            if (!await context.Categories.AnyAsync())
            {
                context.Categories.AddRange(
                    new Category { Name = "ATM Support" },
                    new Category { Name = "Core Banking" },
                    new Category { Name = "Internet Banking" },
                    new Category { Name = "Email & Outlook" },
                    new Category { Name = "Active Directory" },
                    new Category { Name = "Network & WiFi" },
                    new Category { Name = "Desktop & Laptop" },
                    new Category { Name = "Printer Support" },
                    new Category { Name = "CCTV & Security" },
                    new Category { Name = "Software Installation" }
                );
            }

            await context.SaveChangesAsync();
        }
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "admin@bankitdesk.com";
            string adminPassword = "Admin@123";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
        public static async Task SeedManagerUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Seed multiple manager accounts for automatic assignment
            var managers = new[]
            {
                new { Email = "manager1@bankitdesk.com", Password = "Manager@123", FullName = "ATM Support Manager" },
                new { Email = "manager2@bankitdesk.com", Password = "Manager@123", FullName = "Core Banking Manager" },
                new { Email = "manager3@bankitdesk.com", Password = "Manager@123", FullName = "Network Manager" }
            };

            foreach (var m in managers)
            {
                var existing = await userManager.FindByEmailAsync(m.Email);
                if (existing == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = m.Email,
                        Email = m.Email,
                        FullName = m.FullName,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, m.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Manager");
                    }
                }
            }
        }
    }
}