using Graduation_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Graduation_Project.Controllers
{
    public class SupplierOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public SupplierOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> rolemanager, SignInManager<ApplicationUser> signin)
        {
            _context = context;
            _userManager = usermanager;
            _roleManager = rolemanager;
            _signInManager = signin;
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            // Get the order and its related items
            var supplierOrder = await _context.SupplierOrders
                .Include(o => o.SupplierOrderItems)
                    .ThenInclude(oi => oi.SupplierMedication)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (supplierOrder == null || supplierOrder.orderStatus != "Delivered")
            {
                return NotFound("Order not found or not delivered yet.");
            }

            // Get the pharmacist's inventory
            var pharmacistInventory = await _context.Inventory
                .FirstOrDefaultAsync(i => i.BranchId == supplierOrder.BranchId);

            if (pharmacistInventory == null)
            {
                return NotFound("Pharmacist inventory not found.");
            }

            // Process each item in the order
            foreach (var item in supplierOrder.SupplierOrderItems)
            {
                var supplierMedication = item.SupplierMedication;

                if (supplierMedication != null)
                {
                    // Check if the medicine already exists in the pharmacist's inventory
                    var existingMedicine = await _context.Medicines
                        .FirstOrDefaultAsync(m => m.SupplierMedicationId == supplierMedication.SupplierMedicationId);

                    if (existingMedicine != null)
                    {
                        // Update existing medicine (e.g., stock quantity)
                        existingMedicine.ExpiryDate = supplierMedication.ExpiryDate > existingMedicine.ExpiryDate
                            ? supplierMedication.ExpiryDate
                            : existingMedicine.ExpiryDate; // Keep the latest expiry date
                        existingMedicine.StockQuantity += item.Quantity; // Increment stock quantity
                    }
                    else
                    {
                        // Add a new medicine entry
                        var newMedicine = new Medicine
                        {
                            Name = supplierMedication.Name,
                            StockQuantity = supplierMedication.StockQuantity,
                            Description = supplierMedication.Name,
                            HowToUse = "Follow prescription", // Default usage instructions
                            ImageUrl = "/images/product_03.png", // Set if an image is available
                            ExpiryDate = supplierMedication.ExpiryDate,
                            InventoryId = pharmacistInventory.InventoryId,
                            SupplierMedicationId = supplierMedication.SupplierMedicationId,
                            GroupMedicineId = null, // Set group ID if applicable
                            Inventory  = pharmacistInventory,
                            


                        };

                        _context.Medicines.Add(newMedicine);
                    }
                }
            }

            // Mark the order as "Accepted"
            supplierOrder.orderStatus = "Accepted";
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacistOrders" , "Pharmacist"); // Redirect to the Orders view after accepting the order
        }


    }
}
