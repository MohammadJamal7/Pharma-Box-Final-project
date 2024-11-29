using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    // Full name of the user
    public string FullName { get; set; }

    // Address is relevant for Patient and optionally Pharmacist
    public string? Address { get; set; } // Nullable for users who do not need it (e.g., Pharmacists)

    // UserType is used to differentiate roles (consider using roles instead for access control)
    public string UserType { get; set; } // Consider using Identity roles for role-based access



    // relationships :

    // Nullable BranchId since only Pharmacists have a branch
    public int? BranchId { get; set; }  // Nullable for non-Pharmacist users
    [ForeignKey("BranchId")]
    public Branch Branch { get; set; }  // Branch for Pharmacists
    // Navigation properties
    public ICollection<ChatMessage> ChatMessages { get; set; }  // Messages for all users
    public List<Order> SupplierAndUserOrders { get; set; }  // Orders mostly relevant for Patients, but can be used for others too  ... from users to our branch and from pharmacist to suppliers
}
