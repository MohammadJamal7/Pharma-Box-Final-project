using Graduation_Project.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class SupplierOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string supplierId { get; set; }
    public string orderStatus { get; set; }
    // relationships :
    public string PharmacistId { get; set; }  // Foreign Key to ApplicationUser
    [ForeignKey("PharmacistId")]
    public ApplicationUser Pharmacist { get; set; }

    public int BranchId { get; set; }
    [ForeignKey("BranchId")]
    public Branch Branch { get; set; }
    public virtual ICollection<SupplierOrderItem> SupplierOrderItems { get; set; }
}

public class SupplierOrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public int SupplierOrderId { get; set; }
    public virtual SupplierOrder SupplierOrder { get; set; }

    public int? SupplierMedicationId { get; set; }
    public virtual SupplierMedication SupplierMedication { get; set; }
}
