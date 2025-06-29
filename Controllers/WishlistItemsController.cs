using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lavender_Veil.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Lavender_Veil.Controllers
{
    [Authorize]
    public class WishlistItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;

        public WishlistItemsController(ApplicationDbContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WishlistItems
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var items = _context.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.CustomerId == user.Id)
                .ToList();

            return View(items);
        }

        // GET: WishlistItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Customer)
                .Include(w => w.Product)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // GET: WishlistItems/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        // POST: WishlistItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WishlistItemId,ProductId,CustomerId,AddedAt")] WishlistItem wishlistItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wishlistItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", wishlistItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", wishlistItem.ProductId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", wishlistItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", wishlistItem.ProductId);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WishlistItemId,ProductId,CustomerId,AddedAt")] WishlistItem wishlistItem)
        {
            if (id != wishlistItem.WishlistItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishlistItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishlistItemExists(wishlistItem.WishlistItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", wishlistItem.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name", wishlistItem.ProductId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Customer)
                .Include(w => w.Product)
                .FirstOrDefaultAsync(m => m.WishlistItemId == id);
            if (wishlistItem == null)
            {
                return NotFound();
            }

            return View(wishlistItem);
        }

        // POST: WishlistItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishlistItem = await _context.WishlistItems.FindAsync(id);
            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishlistItemExists(int id)
        {
            return _context.WishlistItems.Any(e => e.WishlistItemId == id);
        }
        [HttpGet]
        public async Task<IActionResult> AddToWishlist(Guid productId)
        {
            var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : "guest";

            var exists = _context.WishlistItems.Any(w => w.ProductId == productId && w.CustomerId == userId);
            if (!exists)
            {
                var wishlistItem = new WishlistItem
                {
                    ProductId = productId,
                    CustomerId = userId,
                    AddedAt = DateTime.Now
                };
                _context.WishlistItems.Add(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistItemId)
        {
            var item = await _context.WishlistItems.FindAsync(wishlistItemId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
