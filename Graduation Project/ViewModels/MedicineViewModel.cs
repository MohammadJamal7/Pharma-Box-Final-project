using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.ViewModels
{
    public class MedicineViewModel
    {
        public string Name { get; set; }
        public string HowToUse { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? GroupMedicineId { get; set; }
        public string? GroupMedicineName { get; set; }

        public string? ExistingImagePath { get; set; }
        public bool RequiresPrescription { get; set; }






        [Display(Name = "Upload Image")]
            public IFormFile? ImageFile { get; set; } // For file uploads
        

    }
}
