using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lavender_Veil.Models; 
using Microsoft.EntityFrameworkCore;

namespace Lavender_Veil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Messages()
        {
            var messages = _context.ContactMessages.OrderByDescending(m => m.SentAt).ToList();
            return View(messages);
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.OrdersCompleted = await _context.Orders.CountAsync(o => o.Status == "Completed");
            ViewBag.OrdersPending = await _context.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalSales = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SelectMany(o => o.OrderItems)
                .SumAsync(i => i.Product.Price * i.Quantity); // افتراضيًا يوجد جدول Items

            ViewBag.CategoryStock = await _context.Products
                .GroupBy(p => p.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.SizeStock = await _context.Products
                .GroupBy(p => p.Size.Name)
                .Select(g => new { Size = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.WishlistCount = await _context.WishlistItems.CountAsync();
            ViewBag.CartItemCount = await _context.CartItems.CountAsync();

            return View();
        }
        public async Task<IActionResult> OrderDetails(string id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> Users(string searchTerm)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u =>
                    u.UserName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.Name.Contains(searchTerm));
            }

            var users = await usersQuery.ToListAsync();
            return View(users);
        }


        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Orders(string statusFilter)
        {
            var ordersQuery = _context.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == statusFilter);
            }

            var orders = await ordersQuery.ToListAsync();
            return View(orders);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string newStatus)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                    return NotFound(new { message = "Order not found" });

                order.Status = newStatus;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.Customer)
                .ToListAsync();

            return View(payments);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(string paymentId, string newStatus)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment != null)
            {
                payment.Status = newStatus;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Payments");
        }

    }
}
