using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lavender_Veil.Models;
using Microsoft.AspNetCore.Authorization;

namespace Lavender_Veil.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId, int? sizeId, string search)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (sizeId.HasValue)
                query = query.Where(p => p.SizeId == sizeId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            var products = await query.ToListAsync();

            // إذا كان الطلب AJAX، نعيد فقط Partial View بدون الصفحة الكاملة
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FilteredProducts", products);
            }

            // نمرر القيم للفلاتر (للصفحة الكاملة)
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewBag.Sizes = new SelectList(_context.Sizes, "SizeId", "Name");
            ViewBag.CurrentSearch = search;

            return View(products);
        }
        public IActionResult Payment()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Shop");

            var results = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Where(p =>
                    p.Name.Contains(query) ||
                    (p.Description != null && p.Description.Contains(query)) ||
                    p.Category.Name.Contains(query)
                )
                .ToListAsync();

            // ✅ حل الخطأ: تجهيز الـ ViewBag
            ViewBag.categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            ViewBag.Sizes = new SelectList(await _context.Sizes.ToListAsync(), "SizeId", "Name");

            return View("Shop", results);
        }



        public async Task<IActionResult> ByCategory(int categoryId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            var category = await _context.Categories.FindAsync(categoryId);
            ViewBag.CategoryName = category?.Name ?? "All";

            return View(products);
        }
        public async Task<IActionResult> Shop(int? categoryId, int? sizeId, string search)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .AsQueryable();

            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);

            if (sizeId.HasValue)
                productsQuery = productsQuery.Where(p => p.SizeId == sizeId);

            if (!string.IsNullOrWhiteSpace(search))
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewBag.Sizes = new SelectList(_context.Sizes, "SizeId", "Name");
            ViewBag.CurrentSearch = search;

            return View(await productsQuery.ToListAsync());
        }



        public async Task<IActionResult> Details(Guid? productId)
        {
            if (productId == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return NotFound();

            // عدد مرات شراء المنتج الحالي
            var purchaseCount = await _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .SumAsync(oi => oi.Quantity);

            ViewBag.PurchaseCount = purchaseCount;

            // جلب المنتجات المشابهة (نفس الفئة) والمباعة ≥ 4 مرات
            var similarProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != productId)
                .ToListAsync();

            var filteredSimilar = new List<Product>();

            foreach (var p in similarProducts)
            {
                var count = await _context.OrderItems
                    .Where(oi => oi.ProductId == p.ProductId)
                    .SumAsync(oi => oi.Quantity);

                if (count >= 4)
                    filteredSimilar.Add(p);
            }

            ViewBag.SimilarProducts = filteredSimilar;

            return View(product);
        }


        [HttpGet]
        public IActionResult GetCategoryImages(int categoryId)
        {
            
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                return Json(new List<string>());
            }

            var folderName = category.Name.Replace(" ", "");
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "product", folderName);

            if (!Directory.Exists(imagesPath))
            {
                return Json(new List<string>());
            }

            var images = Directory.GetFiles(imagesPath)
                                 .Select(Path.GetFileName)
                                 .ToList();

            return Json(images);
        }
        public IActionResult Create()
        {
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "product");
            var existingImages = Directory.GetFiles(imagesPath)
                                        .Select(Path.GetFileName)
                                        .ToList();

            ViewBag.ExistingImages = new SelectList(existingImages);
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewBag.SizeId = new SelectList(_context.Sizes, "SizeId", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string selectedImage)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedImage))
                {
                    var category = await _context.Categories.FindAsync(product.CategoryId);
                    var categoryFolder = category?.Name.Replace(" ", "");
                    product.ImageUrl = $"/Images/product/{categoryFolder}/{selectedImage}";
                }

                if (!ModelState.IsValid)
                {
                    product.ProductId = Guid.NewGuid();
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

             
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "product");
                var existingImages = Directory.GetFiles(imagesPath)
                                            .Select(Path.GetFileName)
                                            .ToList();

                ViewBag.ExistingImages = new SelectList(existingImages);
                ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                ViewBag.SizeId = new SelectList(_context.Sizes, "SizeId", "Name", product.SizeId);
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"ModelState Error - {key}: {error.ErrorMessage}");
                    }
                }

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error" + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }
        [HttpGet("Products/Edit/{productId}")]
        public async Task<IActionResult> Edit(Guid? productId)
        {
            if (productId == null) return NotFound();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "Name", product.SizeId);
            return View(product);
        }

        [HttpPost("Products/Edit/{productId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid productId, Product updatedProduct, IFormFile? ImageFile)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            // تحديث البيانات يدويًا
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Stock = updatedProduct.Stock;
            product.SizeId = updatedProduct.SizeId;
            product.CategoryId = updatedProduct.CategoryId;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/products/" + fileName;
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Error updating product.";
                return View(updatedProduct);
            }
        }

        public async Task<IActionResult> Delete(Guid? productId)
        {
            if (productId == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ProductId == productId);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid productId)
        {
            return _context.Products.Any(e => e.ProductId == productId);
        }
    }
}
