using Graduation_Project.Data;
using Hangfire; // Import Hangfire namespace
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.CommandTimeout(60))); // Set the command timeout to 60 seconds

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity options
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // No email confirmation required
    options.Tokens.AuthenticatorTokenProvider = null; // Disable 2FA token provider
})
    .AddRoles<IdentityRole>()  // Add role management support
    .AddEntityFrameworkStores<ApplicationDbContext>();  // Use ApplicationDbContext for Identity

// Add Hangfire services
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString)); // Use the same database connection
builder.Services.AddHangfireServer(); // Add Hangfire background job server

// Build the app
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

// Add Hangfire middleware
app.UseHangfireDashboard(); // Enables the dashboard at "/hangfire"
app.UseHangfireServer();    // Starts the Hangfire server

// Enable routing and authorization
app.UseRouting();
app.UseAuthorization();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pharmacist}/{action=Login}/{id?}");
app.MapRazorPages();

// Run the application
app.Run();
