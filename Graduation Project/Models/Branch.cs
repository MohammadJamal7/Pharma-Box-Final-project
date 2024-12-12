using Graduation_Project.Models;
using System.ComponentModel.DataAnnotations;

public class Branch
{
    [Key]
    public int BranchId { get; set; }

    [Required(ErrorMessage = "Branch Name is required.")]
    [StringLength(100, ErrorMessage = "Branch Name must be at most 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(200, ErrorMessage = "Location must be at most 200 characters.")]
    public string Location { get; set; }



    [Required(ErrorMessage = "Contact number is required.")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Contact number must be between 10 and 15 characters.")]
    [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Contact number must be a valid phone number. Optional country code is allowed.")]
    public string ContactNumber { get; set; }

    //public ICollection<Order> Orders { get; set; }
    //public ICollection<SupplierOrder> SupplierOrders { get; set; }



    // relationships :

    // a branch will have list of suppliers , list of user orders , and will have only one admin (the pharmacist) , and only one inventory!
    public List<ApplicationUser> ?suppliers { get; set; } 
    public List<Order> ?patientOrders { get; set; }
    public Inventory ?Inventory { get; set; }      // One-to-one relationship with Inventory

   // public ApplicationUser adminBranch { get; set; }       // One-to-one relationship with user


}
