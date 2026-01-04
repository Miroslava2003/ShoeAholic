using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;
using ShoeAholic.Models;
using System.Security.Claims;

namespace ShoeAholic.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public CartController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.ShoppingCartItems
                .Include(x => x.Shoe)
                    .ThenInclude(s => s.Images)
                .Include(x => x.ShoeSize)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int shoeId, int sizeId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                         Request.Headers["Accept"].ToString().Contains("application/json");

            var size = await _context.ShoeSizes
                .FirstOrDefaultAsync(s => s.Id == sizeId && s.ShoeId == shoeId);

            if (size == null)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "Избраният размер не съществува." });
                }
                TempData["Error"] = "Избраният размер не съществува.";
                return RedirectToAction("Details", "Products", new { id = shoeId });
            }

            if (size.Quantity < quantity)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = $"Няма достатъчно наличност. Налични: {size.Quantity} бр." });
                }
                TempData["Error"] = $"Няма достатъчно наличност. Налични: {size.Quantity} бр.";
                return RedirectToAction("Details", "Products", new { id = shoeId });
            }

            var existingItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ShoeId == shoeId &&
                    x.SizeId == sizeId);

            if (existingItem != null)
            {
                if (size.Quantity < existingItem.Quantity + quantity)
                {
                    if (isAjax)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Не можете да добавите повече. Налични: {size.Quantity} бр., в количката: {existingItem.Quantity} бр."
                        });
                    }
                    TempData["Error"] = $"Не можете да добавите повече. Налични: {size.Quantity} бр., в количката: {existingItem.Quantity} бр.";
                    return RedirectToAction("Details", "Products", new { id = shoeId });
                }

                existingItem.Quantity += quantity;
            }
            else
            {
                _context.ShoppingCartItems.Add(new ShoppingCartItem
                {
                    UserId = userId,
                    ShoeId = shoeId,
                    SizeId = sizeId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();

            if (isAjax)
            {
                return Json(new { success = true, message = "Продуктът е добавен в количката!" });
            }

            TempData["Success"] = "Продуктът е добавен в количката!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.ShoppingCartItems
                .Include(x => x.ShoeSize)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item == null)
                return NotFound();

            if (quantity <= 0)
            {
                _context.ShoppingCartItems.Remove(item);
                TempData["Success"] = "Продуктът е премахнат от количката.";
            }
            else
            {
                if (item.ShoeSize != null && quantity > item.ShoeSize.Quantity)
                {
                    TempData["Error"] = $"Няма достатъчно наличност. Максимум: {item.ShoeSize.Quantity} бр.";
                    return RedirectToAction("Index");
                }

                item.Quantity = quantity;
                TempData["Success"] = "Количеството е обновено.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Продуктът е премахнат от количката!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.ShoppingCartItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Количката е изчистена!";
            return RedirectToAction("Index");
        }

        public async Task<int> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _context.ShoppingCartItems
                .Where(x => x.UserId == userId)
                .SumAsync(x => x.Quantity);
        }
    }
}