namespace Graduation_Project.Models
{
    public class PharmCartItem
    {
        public int Id { get; set; }  // Unique ID for each cart item
        public int MedicationId { get; set; }  // Foreign Key to Medicine
        public SupplierMedication Medication { get; set; }  // Navigation property to Medicine
        public int Quantity { get; set; }  // Quantity of the medication
        public int PharmCartId { get; set; }  // Foreign Key to PharmCart
        public PharmCart PharmCart { get; set; }  // Navigation property to PharmCart
    }
}
