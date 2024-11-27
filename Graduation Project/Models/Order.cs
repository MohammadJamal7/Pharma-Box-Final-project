namespace Graduation_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Foreign Key to ApplicationUser
        public ApplicationUser User { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DeliveredDate { get; set; }  // When the order is delivered
        public string PharmacyId { get; set; } // To associate with a pharmacy
    }
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int MedicineId { get; set; }
        public Medicine Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }


    public enum OrderStatus
    {
        Pending,
        Processed,
        Shipped,
        Delivered,
        Cancelled
    }
}
