using Graduation_Project.Models;
using System;
using System.ComponentModel.DataAnnotations;

    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }
        public string HowToUse { get; set; }
        public string ImageUrl { get; set; }  
        public DateTime ExpiryDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

}
