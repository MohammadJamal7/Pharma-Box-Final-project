using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
            public DbSet<OrderNotifications> OrderNotifications { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);

                // Branch and Inventory (One-to-One with Cascade Delete)
                builder.Entity<Branch>()
                    .HasOne(b => b.Inventory)
                    .WithOne(i => i.Branch)
                    .HasForeignKey<Inventory>(i => i.BranchId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ApplicationUser and Branch (One-to-Many with SetNull on Delete)
                builder.Entity<ApplicationUser>()
                    .HasOne(u => u.Branch)
                    .WithMany(b => b.suppliers)
                    .HasForeignKey(u => u.BranchId)
                    .OnDelete(DeleteBehavior.SetNull);

                // ChatMessage Relationships (Restrict Delete)
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

                // PharmCart and PharmCartItem (One-to-Many with Cascade Delete)
                builder.Entity<PharmCart>()
                    .HasMany(p => p.CartItems)
                    .WithOne(c => c.PharmCart)
                    .HasForeignKey(c => c.PharmCartId)
                    .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between PharmCartItem and Medicine
            builder.Entity<PharmCartItem>()
                .HasOne(c => c.Medication)  // Each PharmCartItem has one Medication
                .WithMany()  // Medication can be in many PharmCartItems
                .HasForeignKey(c => c.MedicationId)  // Foreign key in PharmCartItem
                .OnDelete(DeleteBehavior.Restrict);  // Avoid cascading deletes for Medicines

            // Order and OrderItem (Restrict Delete)
            builder.Entity<OrderItem>()
                    .HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // OrderItem and Medicine (Restrict Delete)
                builder.Entity<OrderItem>()
                    .HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict);

                // OrderNotifications (Restrict Delete)
                builder.Entity<OrderNotifications>()
                    .HasOne(n => n.SupplierOrder)
                    .WithMany()
                    .HasForeignKey(n => n.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<OrderNotifications>()
                    .HasOne(n => n.Pharmacist)
                    .WithMany()
                    .HasForeignKey(n => n.PharmacistId)
                    .OnDelete(DeleteBehavior.Restrict);

               
                // Inventory and Medicines (Cascade Delete)
                builder.Entity<Inventory>()
                    .HasMany(i => i.Medicines)
                    .WithOne(m => m.Inventory)
                    .HasForeignKey(m => m.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SupplierOrder>()
       .HasMany(so => so.SupplierOrderItems)
       .WithOne(soi => soi.SupplierOrder)
       .HasForeignKey(soi => soi.SupplierOrderId)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SupplierOrderItem>()
       .HasOne(soi => soi.SupplierOrder)
       .WithMany(so => so.SupplierOrderItems)
       .HasForeignKey(soi => soi.SupplierOrderId)
       .OnDelete(DeleteBehavior.Cascade);
        }
        


    }
}
