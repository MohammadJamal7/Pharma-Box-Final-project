using Graduation_Project.Models;
using System.ComponentModel.DataAnnotations;

public class Branch
{
    [Key]
    public int BranchId { get; set; }

    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    public string Location { get; set; }

    //public ICollection<Order> Orders { get; set; }
    //public ICollection<SupplierOrder> SupplierOrders { get; set; }



    // relationships :

    // a branch will have list of suppliers , list of user orders , and will have only one admin (the pharmacist) , and only one inventory!
    public List<ApplicationUser> suppliers { get; set; } 
    public List<Order> patientOrders { get; set; }
    public Inventory Inventory { get; set; }      // One-to-one relationship with Inventory

   // public ApplicationUser adminBranch { get; set; }       // One-to-one relationship with user


}
