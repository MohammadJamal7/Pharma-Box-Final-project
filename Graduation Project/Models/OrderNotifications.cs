using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class OrderNotifications
    {


        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("SupplierOrder")]
        public int ? OrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }  // Navigation property for SupplierOrder

        [Required]
        [ForeignKey("Pharmacist")]
        public string PharmacistId { get; set; }
        public ApplicationUser Pharmacist { get; set; }  // Assuming "User" represents pharmacists

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime NotificationDate { get; set; } = DateTime.Now;

       

        [Required]
        [StringLength(50)]
        public string NotificationType { get; set; }  // E.g., "Order Accepted", "Order Delivered"

    }
}
