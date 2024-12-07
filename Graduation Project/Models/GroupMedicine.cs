using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class GroupMedicine
    {
        [Key]
        public int GroupMedicineId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        // Navigation property for the related Medicines
        public List<Medicine> Medicines { get; set; }
    }
}
