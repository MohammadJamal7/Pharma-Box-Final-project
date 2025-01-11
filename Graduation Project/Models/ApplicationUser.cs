using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    // Full name of the user
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    public string FullName { get; set; }

    // Address is relevant for Patient and optionally Pharmacist
    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    public string? Address { get; set; }

    // UserType is used to differentiate roles (consider using roles instead for access control)
    [Required(ErrorMessage = "User Type is required.")]
    public string UserType { get; set; }



    // relationships :

    // Nullable BranchId since only Pharmacists have a branch
    public int? BranchId { get; set; }  // Nullable for non-Pharmacist users
    [ForeignKey("BranchId")]
    public Branch ?Branch { get; set; }  // Branch for Pharmacists
    // Navigation properties
    public ICollection<ChatMessage> ?  ChatMessages { get; set; }  // Messages for all users
    public List<Order> ?  SupplierAndUserOrders { get; set; }  // Orders mostly relevant for Patients, but can be used for others too  ... from users to our branch and from pharmacist to suppliers
    public ICollection<SupplierMedication>? SupplierMedication { get; set; }
}
