using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Full name of the user
    public string FullName { get; set; }

    // Address is relevant for Patient and optionally Pharmacist
    public string? Address { get; set; } // Nullable for users who do not need it (e.g., Pharmacists)

    // Nullable BranchId since only Pharmacists have a branch
    public int? BranchId { get; set; }  // Nullable for non-Pharmacist users
    public Branch Branch { get; set; }  // Branch for Pharmacists

    // UserType is used to differentiate roles (consider using roles instead for access control)
    public string UserType { get; set; } // Consider using Identity roles for role-based access

    // Navigation properties
    public ICollection<ChatMessage> ChatMessages { get; set; }  // Messages for all users
    public ICollection<Order> Orders { get; set; }  // Orders mostly relevant for Patients, but can be used for others too
}
