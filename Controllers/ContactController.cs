using Microsoft.AspNetCore.Mvc;
using Lavender_Veil.Models;
using Microsoft.AspNetCore.Authorization;

namespace Lavender_Veil.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Contact()
        {
            // Pass previous input values back to the view if they exist
            ViewBag.PrevName = TempData["PrevName"] ?? "";
            ViewBag.PrevEmail = TempData["PrevEmail"] ?? "";
            ViewBag.PrevMessage = TempData["PrevMessage"] ?? "";
            
            return View();
        }
        [HttpPost]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            // Store values in case we need to redirect back
            TempData["PrevName"] = Name;
            TempData["PrevEmail"] = Email;
            TempData["PrevMessage"] = Message;

            // Manual validation
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Message))
            {
                TempData["Error"] = "All fields are required!";
                return RedirectToAction("Contact");
            }

            if (!Email.Contains("@"))
            {
                TempData["Error"] = "Invalid email format!";
                return RedirectToAction("Contact");
            }

            // Save to database
            var message = new ContactMessage
            {
                Name = Name,
                Email = Email,
                Message = Message,
                SentAt = DateTime.Now
            };

            _context.ContactMessages.Add(message);
            _context.SaveChanges();
            
            // Clear temp data if successful
            TempData.Remove("PrevName");
            TempData.Remove("PrevEmail");
            TempData.Remove("PrevMessage");
            
            TempData["ContactSuccess"] = "Message sent successfully!";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")] // Optional: Restrict to admins
        public IActionResult Messages()
        {
            var messages = _context.ContactMessages
                .OrderByDescending(m => m.SentAt)
                .ToList();
            return View(messages);
        }
    }
}