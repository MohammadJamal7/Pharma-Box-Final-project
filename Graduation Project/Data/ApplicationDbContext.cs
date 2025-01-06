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
        public DbSet<PharmCart> PharmCarts { get; set; }
        public DbSet<PharmCartItem> PharmCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the one-to-one relationship between branch and inventory
            builder.Entity<Branch>()
                .HasOne(b => b.Inventory)
                .WithOne(i => i.Branch)
                .HasForeignKey<Inventory>(i => i.BranchId)
                .OnDelete(DeleteBehavior.Cascade);  // Enabled Cascade Delete

            // Configure the relationship between ApplicationUser and Branch
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.suppliers)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure the relationship between ApplicationUser and ChatMessages
            builder.Entity<ChatMessage>()
                .HasOne(c => c.Sender)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChatMessage>()
                .HasOne(c => c.Receiver)
                .WithMany()
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between PharmCart and PharmCartItem
            builder.Entity<PharmCart>()
                .HasMany(p => p.CartItems)
                .WithOne(c => c.PharmCart)
                .HasForeignKey(c => c.PharmCartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between PharmCartItem and Medicine
            builder.Entity<PharmCartItem>()
                .HasOne(c => c.Medication)
                .WithMany()
                .HasForeignKey(c => c.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between Order and OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between OrderItem and Medicine
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between Branch and Inventory (Cascade Delete)
            builder.Entity<Branch>()
                .HasOne(b => b.Inventory)
                .WithOne(i => i.Branch)
                .HasForeignKey<Inventory>(i => i.BranchId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade Inventory when Branch is deleted

            // Configure the relationship between Inventory and Medicines (Cascade Delete)
            builder.Entity<Inventory>()
                .HasMany(i => i.Medicines)
                .WithOne(m => m.Inventory)
                .HasForeignKey(m => m.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade Medicines when Inventory is deleted



        }


    }
}
