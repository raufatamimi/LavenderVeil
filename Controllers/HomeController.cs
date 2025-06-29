using System.Diagnostics;
using Lavender_Veil.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lavender_Veil.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
        .Include(p => p.Category)
        .Include(p => p.Size)
        .Take(8) 
        .ToList();

            return View(products);
        }
        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Support()
        {
            return View();
        }

        public IActionResult Tos()
        {
            return View(); 
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Frequency()
        {
            var data = _context.OrderItems
                .Include(o => o.Product)
                .GroupBy(o => new { o.Product.Name, o.Product.ImageUrl })
                .Where(g => g.Sum(x => x.Quantity) >= 4)
                .Select(g => new ProductFrequencyViewModel
                {
                    ProductName = g.Key.Name,
                    ProductImageUrl = g.Key.ImageUrl,
                    PurchaseCount = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(p => p.PurchaseCount)
                .ToList();

            return View(data);
        }
        public IActionResult Recommendations()
        {
            var recommendedProducts = _context.OrderItems
                .Include(o => o.Product)
                .GroupBy(o => o.Product)
                .Where(g => g.Sum(x => x.Quantity) >= 4)
                .Select(g => g.Key) // المنتج نفسه
                .ToList();

            return View(recommendedProducts);
        }
    }
}
