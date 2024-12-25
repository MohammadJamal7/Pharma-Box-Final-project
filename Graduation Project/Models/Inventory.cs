using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }
    public int QuantityAvailable { get; set; }


    // relationships (with branch (one to one) and with medicine (one to many) )
    public int ? BranchId { get; set; }
    [ForeignKey("BranchId")]
    public Branch ? Branch { get; set; }

    public List<Medicine> ? Medicines { get; set; }

}
