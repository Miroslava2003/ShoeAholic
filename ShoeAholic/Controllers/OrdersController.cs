using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Areas.Identity.Data;
using ShoeAholic.Data;
using ShoeAholic.Models;
using ShoeAholic.Models.Enums;
using ShoeAholic.Models.ViewModels;
using System.Security.Claims;

namespace ShoeAholic.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ShoeAholicDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ShoeAholicDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.ShoppingCartItems
                .Include(x => x.Shoe)
                .Include(x => x.ShoeSize)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Количката ви е празна.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var subTotal = cartItems.Sum(x => (x.Shoe?.Price ?? 0) * x.Quantity);

            var model = new CheckoutViewModel
            {
                FullName = $"{user.Име} {user.Фамилия}",
                Email = user.Email!,
                Phone = user.Телефон,
                SubTotal = subTotal,
                DeliveryPrice = 0,
                TotalPrice = subTotal,
                ItemsCount = cartItems.Sum(x => x.Quantity)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.ShoppingCartItems
                .Include(x => x.Shoe)
                .Include(x => x.ShoeSize)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Количката ви е празна.";
                return RedirectToAction("Index", "Cart");
            }

            var subTotal = cartItems.Sum(x => (x.Shoe?.Price ?? 0) * x.Quantity);
            decimal deliveryPrice = subTotal < 200
                ? (model.DeliveryType == DeliveryType.Address ? 7.99m : 5.99m)
                : 0;
            var totalPrice = subTotal + deliveryPrice;

            var order = new Order
            {
                UserId = userId!,
                CreatedOn = DateTime.Now,
                SubTotal = subTotal,
                DeliveryPrice = deliveryPrice,
                TotalPrice = totalPrice,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                City = model.City,
                PostalCode = model.PostalCode,
                Address = model.Address,
                DeliveryType = model.DeliveryType,
                PaymentType = model.PaymentType,
                Status = OrderStatus.Pending,
                IsPaid = false
            };

            foreach (var item in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ShoeId = item.ShoeId,
                    Size = item.ShoeSize?.Size.ToString() ?? "N/A",
                    Quantity = item.Quantity,
                    Price = item.Shoe?.Price ?? 0
                });

                if (item.ShoeSize != null)
                    item.ShoeSize.Quantity -= item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (model.PaymentType == PaymentType.Card)
                return RedirectToAction("Payment", new { orderId = order.Id });

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        public async Task<IActionResult> Payment(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Поръчката не е намерена.";
                return RedirectToAction("Index", "Home");
            }

            if (order.IsPaid)
            {
                TempData["Error"] = "Поръчката вече е платена.";
                return RedirectToAction("Details", new { id = orderId });
            }

            var model = new PaymentViewModel
            {
                OrderId = order.Id,
                Amount = order.TotalPrice
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == model.OrderId && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Поръчката не е намерена.";
                return RedirectToAction("Index", "Home");
            }

            await Task.Delay(1500);

            order.IsPaid = true;
            order.PaymentTransactionId = Guid.NewGuid().ToString();
            order.Status = OrderStatus.Processing;

            await _context.SaveChangesAsync();

            var cartItems = await _context.ShoppingCartItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Shoe)
                        .ThenInclude(s => s.Images)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Поръчката не е намерена.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedOn)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Shoe)
                        .ThenInclude(s => s.Images)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Поръчката не е намерена.";
                return RedirectToAction("MyOrders");
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Поръчката не е намерена.";
                return RedirectToAction("MyOrders");
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                TempData["Error"] = "Тази поръчка не може да бъде отменена.";
                return RedirectToAction("Details", new { id });
            }

            order.Status = OrderStatus.Cancelled;

            foreach (var item in order.Items)
            {
                var shoeSize = await _context.ShoeSizes
                    .FirstOrDefaultAsync(s => s.ShoeId == item.ShoeId && s.Size.ToString() == item.Size);

                if (shoeSize != null)
                    shoeSize.Quantity += item.Quantity;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Поръчката е отменена успешно.";
            return RedirectToAction("Details", new { id });
        }
    }
}
