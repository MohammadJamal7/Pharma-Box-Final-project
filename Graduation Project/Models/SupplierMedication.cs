using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class SupplierMedication
    {
        [Key]
        public int SupplierMedicationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }  // Price set by the supplier
        public int StockQuantity { get; set; }  // Quantity available from the supplier
        public DateTime ExpiryDate { get; set; }  // Expiry date specific to the supplier's stock


        // relationships :
        
        public string SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public ApplicationUser Supplier { get; set; }  // Reference to the supplier


    }
}
