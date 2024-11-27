using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string Address { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; }
    public ICollection<Order> Orders { get; set; }
}
