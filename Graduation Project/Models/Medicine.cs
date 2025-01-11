using Graduation_Project.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string HowToUse { get; set; }
        public string? ImageUrl { get; set; }  
        public int StockQuantity { get; set; }  
        public DateTime ExpiryDate { get; set; }
        public double Price { get; set; }

        // Add this property
        public bool RequiresPrescription { get; set; }

    // relationships : 

    public int? InventoryId { get; set; }
    [ForeignKey("InventoryId")]
    public Inventory? Inventory { get; set; }

    public List<OrderItem> OrderItems { get; set; }



    // Relationships
    public int? SupplierMedicationId { get; set; }  // This should be nullable in case there's no supplier stock yet
    public SupplierMedication SupplierMedication { get; set; }  // Reference to the supplier's medicine entry

    // Relationship to GroupMedicine
    public int? GroupMedicineId { get; set; }  // Nullable in case some medicines are not in a group
    [ForeignKey("GroupMedicineId")]
    public GroupMedicine GroupMedicine { get; set; }
}
