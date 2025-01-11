using Graduation_Project.Data;
using Graduation_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] OrderViewModel orderData)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid order data" });
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Create new order
            var order = new Order
            {
                BranchId = 1,
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                TotalAmount = orderData.TotalAmount,
                Status = OrderStatus.Pending ,// Assuming you have an enum for OrderStatus
                OrderItems = new List<OrderItem>() 
            };

            // Add order items
            foreach (var item in orderData.Items)
            {
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine == null)
                {
                    return Json(new { success = false, message = $"Medicine with ID {item.MedicineId} not found" });
                }

                order.OrderItems.Add(new OrderItem
                {
                    MedicineId = item.MedicineId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true, orderId = order.Id });
        }
        catch (Exception ex)
        {
            // Log the exception
            return Json(new { success = false, message = "An error occurred while processing your order" });
        }
    }

   
}