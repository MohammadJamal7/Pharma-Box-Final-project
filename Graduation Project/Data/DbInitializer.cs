using Microsoft.AspNetCore.Identity;
using Graduation_Project.Models;

namespace Graduation_Project.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            string[] roleNames = { "Admin", "Pharmacist", "Supplier", "Patient" };

            // Seed roles if they do not exist
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create a default Admin user if not exists
            var adminUser = new ApplicationUser
            {
                UserName = "admin@pharmabox.com",
                Email = "admin@pharmabox.com",
                FullName = "Admin User",
                EmailConfirmed = true,
                UserType = "Admin"
            };

            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");  // Default password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed a default Branch if none exists
            if (!context.PharmacyBranch.Any())
            {
                var defaultBranch = new Branch
                {
                    Name = "Default Pharmacy",
                    Location = "Main Street, City"
                };

                context.PharmacyBranch.Add(defaultBranch);
                await context.SaveChangesAsync();
            }
        }
    }
}
