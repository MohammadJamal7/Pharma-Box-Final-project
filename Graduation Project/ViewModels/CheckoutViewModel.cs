namespace Graduation_Project.ViewModels
{
    public class CheckoutViewModel
    {
        public ApplicationUser currentUser { get; set; }
        public Branch? Branch { get; set; } // Include branch details
        public Cart Cart { get; set; }

    }
}
