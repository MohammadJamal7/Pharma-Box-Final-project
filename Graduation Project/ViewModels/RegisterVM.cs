using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.ViewModels
{
    public class RegisterVM
    {

        [Required]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ?PhoneNumber { get; set; }
        [Required]
        public int BranchId { get; set; }

        public List<Branch> ?Branches { get; set; } // For dropdown
    }
}
