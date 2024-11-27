using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Medicine> Medicines { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<SupplierOrder> SupplierOrders { get; set; }

        public DbSet<SupplierOrderItem> SupplierOrderItems { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Branch> PharmacyBranch { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the relationship between ApplicationUser and ChatMessages
            builder.Entity<ChatMessage>()
                .HasOne(c => c.Sender)  // Each ChatMessage has one Sender (ApplicationUser)
                .WithMany(u => u.ChatMessages)  // A User can have many ChatMessages
                .HasForeignKey(c => c.SenderId)  // SenderId is the FK
                .OnDelete(DeleteBehavior.Restrict);  // Avoid cascading deletes

            builder.Entity<ChatMessage>()
                .HasOne(c => c.Receiver)  // Each ChatMessage has one Receiver (ApplicationUser)
                .WithMany()  // A User (Receiver) does not need to have a collection of ChatMessages
                .HasForeignKey(c => c.ReceiverId)  // ReceiverId is the FK
                .OnDelete(DeleteBehavior.Restrict);  // Avoid cascading deletes
        }
    }
}
