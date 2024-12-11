using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<GroupMedicine> GroupMedicines { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SupplierOrder> SupplierOrders { get; set; }
        public DbSet<SupplierOrderItem> SupplierOrderItems { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Branch> PharmacyBranch { get; set; }
        public DbSet<SupplierMedication> SupplierMedications { get; set; }
        
        // Add DbSet for PharmCart and PharmCartItem
        public DbSet<PharmCart> PharmCarts { get; set; }
        public DbSet<PharmCartItem> PharmCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the one-to-one relationship between branch and inventory
            builder.Entity<Branch>()
                .HasOne(b => b.Inventory)
                .WithOne(i => i.Branch)
                .HasForeignKey<Inventory>(i => i.BranchId);

            // Configure the relationship between ApplicationUser and Branch
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch) // Each ApplicationUser may have one Branch
                .WithMany(b => b.suppliers) // A Branch can have many suppliers
                .HasForeignKey(u => u.BranchId) // Foreign Key on ApplicationUser
                .OnDelete(DeleteBehavior.SetNull); // Allow BranchId to be null for suppliers

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

            // Configure the relationship between PharmCart and PharmCartItem
            builder.Entity<PharmCart>()
                .HasMany(p => p.CartItems)  // PharmCart has many PharmCartItems
                .WithOne(c => c.PharmCart)  // Each PharmCartItem belongs to one PharmCart
                .HasForeignKey(c => c.PharmCartId)  // Foreign key in PharmCartItem
                .OnDelete(DeleteBehavior.Cascade);  // If PharmCart is deleted, delete the items too

            // Configure the relationship between PharmCartItem and Medicine
            builder.Entity<PharmCartItem>()
                .HasOne(c => c.Medication)  // Each PharmCartItem has one Medication
                .WithMany()  // Medication can be in many PharmCartItems
                .HasForeignKey(c => c.MedicationId)  // Foreign key in PharmCartItem
                .OnDelete(DeleteBehavior.Restrict);  // Avoid cascading deletes for Medicines
        }
    }
}
