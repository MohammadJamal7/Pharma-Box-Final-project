using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    [ForeignKey("Branch")]
    public int BranchId { get; set; }
    public Branch Branch { get; set; }
    public int QuantityAvailable { get; set; }
    public ICollection<Medicine> Medications { get; set; }

}
