using Graduation_Project.Models;
using System.ComponentModel.DataAnnotations;

public class Branch
{
    [Key]
    public int BranchId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public string Location { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<SupplierOrder> SupplierOrders { get; set; }

}
