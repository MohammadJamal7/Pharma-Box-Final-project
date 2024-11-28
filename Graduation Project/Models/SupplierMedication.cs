using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class SupplierMedication
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public int SupplierId { get; set; }
        public ApplicationUser Supplier { get; set; }  // Reference to the supplier

        [ForeignKey("Medicine")]
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }  // Reference to the general medicine

        public decimal Price { get; set; }  // Price set by the supplier
        public int StockQuantity { get; set; }  // Quantity available from the supplier

        public DateTime ExpiryDate { get; set; }  // Expiry date specific to the supplier's stock
    
}
}
