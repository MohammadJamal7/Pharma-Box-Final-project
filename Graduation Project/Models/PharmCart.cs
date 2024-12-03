namespace Graduation_Project.Models
{
    public class PharmCart
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // The user ID (pharmacist)
        public ApplicationUser User { get; set; }  // Navigation property to ApplicationUser

        public string? SupplierId { get; set; } // Track the current supplier for the cart
        public List<PharmCartItem> CartItems { get; set; }  // Collection of items in the cart

        public PharmCart()
        {
            CartItems = new List<PharmCartItem>();  // Initialize the list
        }
    }
}
