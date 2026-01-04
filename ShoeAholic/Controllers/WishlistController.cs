using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;
using ShoeAholic.Models;
using System.Security.Claims;

namespace ShoeAholic.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public WishlistController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlistItems = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Shoe)
                    .ThenInclude(s => s.Images)
                .Include(w => w.Shoe)
                    .ThenInclude(s => s.Sizes)
                .OrderByDescending(w => w.AddedOn)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int shoeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ShoeId == shoeId);

            if (exists)
            {
                TempData["Info"] = "Този продукт вече е в любимите ви!";
                return RedirectToAction("Details", "Products", new { id = shoeId });
            }

            var wishlistItem = new WishlistItem
            {
                UserId = userId!,
                ShoeId = shoeId,
                AddedOn = DateTime.Now
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Продуктът е добавен в любимите!";
            return RedirectToAction("Details", "Products", new { id = shoeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item == null)
                return NotFound();

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Продуктът е премахнат от любимите!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveByShoeId(int shoeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ShoeId == shoeId && w.UserId == userId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Продуктът е премахнат от любимите!";
            }

            return RedirectToAction("Details", "Products", new { id = shoeId });
        }

        public async Task<int> GetCount()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return 0;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .CountAsync();
        }
    }
}