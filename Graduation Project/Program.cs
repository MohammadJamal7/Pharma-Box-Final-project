using Graduation_Project.Data;
using Graduation_Project.Models; // Ensure ApplicationUser is included
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Use ApplicationUser for Identity and add Role support
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Add role management support
    .AddEntityFrameworkStores<ApplicationDbContext>();  // Use ApplicationDbContext for Identity

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles, admin user, and default branch if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();  // Correct UserManager
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();  // Correct RoleManager
    var context = services.GetRequiredService<ApplicationDbContext>();  // Access to the ApplicationDbContext

    // Initialize the roles, admin user, and default branch
    await DbInitializer.SeedRolesAndAdminAsync(userManager, roleManager, context); // Pass context for branch seeding
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
