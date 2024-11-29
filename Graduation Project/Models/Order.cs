using System.ComponentModel.DataAnnotations.Schema;

namespace Graduation_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DeliveredDate { get; set; }  // When the order is delivered



        // relationships :

        public string UserId { get; set; }  // Foreign Key to ApplicationUser
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }



        // relationships :
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public int MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public Medicine Product { get; set; }
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
