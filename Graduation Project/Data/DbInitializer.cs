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

            // Create a default Supplier user if not exists
            var supplierUser = new ApplicationUser
            {
                UserName = "supplier@pharmabox.com",
                Email = "supplier@pharmabox.com",
                FullName = "Supplier User",
                EmailConfirmed = true,
                UserType = "Supplier"
            };

            var existingSupplier = await userManager.FindByEmailAsync(supplierUser.Email);
            if (existingSupplier == null)
            {
                var result = await userManager.CreateAsync(supplierUser, "Supplier@123");  // Default password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(supplierUser, "Supplier");
                }
            }



            // Seed a default Branch if none exists
            if (!context.PharmacyBranch.Any())
            {
                var defaultBranch = new Branch
                {
                    Name = "Default Pharmacy",
                    Location = "Main Street, City",
                    ContactNumber = "0777888555"
                };

                context.PharmacyBranch.Add(defaultBranch);
                await context.SaveChangesAsync();

                if (defaultBranch.Inventory==null)
                {
                    //Seed a default Inventory for the Branch
                    var defaultInventory = new Inventory
                    {
                        Branch = defaultBranch,
                        BranchId = defaultBranch.BranchId,

                    };
                    context.Inventory.Add(defaultInventory);
                    await context.SaveChangesAsync();
                }
            }

         
        }
    }
}
