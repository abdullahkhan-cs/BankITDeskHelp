using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BankITDeskHelp.Models;
using BankITDeskHelp.Constants;

namespace BankITDeskHelp.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { Roles.Admin, Roles.Manager, Roles.Employee, Roles.ITStaff };

                foreach (var roleName in roleNames)
                {
                    bool roleExists = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                        if (!result.Succeeded)
                        {
                            Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedRolesAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        public static async Task SeedMasterDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Seed Departments
                if (!await context.Departments.AnyAsync())
                {
                    context.Departments.AddRange(
                        new Department { Name = "ATM Support" },
                        new Department { Name = "Core Banking" },
                        new Department { Name = "Internet Banking" },
                        new Department { Name = "Network & Infrastructure" },
                        new Department { Name = "Software Support" }
                    );
                    await context.SaveChangesAsync();
                    Console.WriteLine("Departments seeded successfully");
                }

                // Seed Branches
                if (!await context.Branches.AnyAsync())
                {
                    context.Branches.AddRange(
                        new Branch { Name = "Main Branch", Location = "Karachi" },
                        new Branch { Name = "Nawabshah Branch", Location = "Nawabshah" },
                        new Branch { Name = "Hyderabad Branch", Location = "Hyderabad" }
                    );
                    await context.SaveChangesAsync();
                    Console.WriteLine("Branches seeded successfully");
                }

                // Seed Categories
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
                    await context.SaveChangesAsync();
                    Console.WriteLine("Categories seeded successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedMasterDataAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            try
            {
                string adminEmail = "admin@bankitdesk.com";
                string adminPassword = configuration["SeedData:AdminPassword"] ?? "Admin@123";

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
                        var roleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                        if (roleResult.Succeeded)
                        {
                            Console.WriteLine($"Admin user created successfully: {adminEmail}");
                        }
                        else
                        {
                            Console.WriteLine($"Error assigning admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"Admin user already exists: {adminEmail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedAdminUserAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        public static async Task SeedManagerUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            try
            {
                string managerPassword = configuration["SeedData:ManagerPassword"] ?? "Manager@123";

                var managers = new[]
                {
                    new { Email = "atmmanager@bankitdesk.com", FullName = "ATM Support Manager", DepartmentName = "ATM Support" },
                    new { Email = "corebankingmanager@bankitdesk.com", FullName = "Core Banking Manager", DepartmentName = "Core Banking" },
                    new { Email = "internetbankingmanager@bankitdesk.com", FullName = "Internet Banking Manager", DepartmentName = "Internet Banking" },
                    new { Email = "networkingmanager@bankitdesk.com", FullName = "Network & Infrastructure Manager", DepartmentName = "Network & Infrastructure" },
                    new { Email = "softwaremanager@bankitdesk.com", FullName = "Software Support Manager", DepartmentName = "Software Support" }
                };

                foreach (var m in managers)
                {
                    try
                    {
                        var existing = await userManager.FindByEmailAsync(m.Email);
                        if (existing == null)
                        {
                            var department = await context.Departments.FirstOrDefaultAsync(d => d.Name == m.DepartmentName);
                            if (department == null)
                            {
                                Console.WriteLine($"Warning: Department '{m.DepartmentName}' not found for manager {m.Email}");
                            }

                            var user = new ApplicationUser
                            {
                                UserName = m.Email,
                                Email = m.Email,
                                FullName = m.FullName,
                                DepartmentId = department?.Id,
                                EmailConfirmed = true
                            };

                            var result = await userManager.CreateAsync(user, managerPassword);
                            if (result.Succeeded)
                            {
                                var roleResult = await userManager.AddToRoleAsync(user, Roles.Manager);
                                if (roleResult.Succeeded)
                                {
                                    Console.WriteLine($"Manager user created successfully: {m.Email}");
                                }
                                else
                                {
                                    Console.WriteLine($"Error assigning manager role to {m.Email}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Error creating manager user {m.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Manager user already exists: {m.Email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating manager {m.Email}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedManagerUserAsync: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}