using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;
using ShoeAholic.Models;
using ShoeAholic.Models.Enums;
using ShoeAholic.Models.ViewModels;

namespace ShoeAholic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public AdminOrdersController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddMonths(-1);

            var stats = new AdminOrdersStatsViewModel
            {
                TodayOrders = await _context.Orders.Where(o => o.CreatedOn.Date == today).CountAsync(),
                TodayRevenue = await _context.Orders.Where(o => o.CreatedOn.Date == today).SumAsync(o => (decimal?)o.TotalPrice) ?? 0,
                WeekOrders = await _context.Orders.Where(o => o.CreatedOn >= weekAgo).CountAsync(),
                WeekRevenue = await _context.Orders.Where(o => o.CreatedOn >= weekAgo).SumAsync(o => (decimal?)o.TotalPrice) ?? 0,
                MonthOrders = await _context.Orders.Where(o => o.CreatedOn >= monthAgo).CountAsync(),
                MonthRevenue = await _context.Orders.Where(o => o.CreatedOn >= monthAgo).SumAsync(o => (decimal?)o.TotalPrice) ?? 0,
                TotalOrders = await _context.Orders.CountAsync(),
                TotalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalPrice) ?? 0,
                PendingOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Pending).CountAsync(),
                ProcessingOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Processing).CountAsync(),
                ShippedOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Shipped).CountAsync(),
                DeliveredOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Delivered).CountAsync(),
                CancelledOrders = await _context.Orders.Where(o => o.Status == OrderStatus.Cancelled).CountAsync(),
                AverageOrderValue = await _context.Orders.AnyAsync() ? await _context.Orders.AverageAsync(o => o.TotalPrice) : 0
            };

            return View(stats);
        }

        public async Task<IActionResult> Index(string search, OrderStatus? status, DateTime? dateFrom, DateTime? dateTo, int pageSize = 20, int page = 1)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Shoe)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.FullName.Contains(search) ||
                    o.Phone.Contains(search) ||
                    o.Address.Contains(search));
            }

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (dateFrom.HasValue)
                query = query.Where(o => o.CreatedOn.Date >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(o => o.CreatedOn.Date <= dateTo.Value.Date);

            query = query.OrderByDescending(o => o.CreatedOn);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            ViewData["Search"] = search;
            ViewData["Status"] = status;
            ViewData["DateFrom"] = dateFrom?.ToString("yyyy-MM-dd");
            ViewData["DateTo"] = dateTo?.ToString("yyyy-MM-dd");

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Shoe)
                        .ThenInclude(s => s.Images)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Статусът на поръчка #{id} е променен на {GetStatusDisplayName(status)}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Shoe)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (id != model.Id)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.FullName = model.FullName;
            order.Phone = model.Phone;
            order.Address = model.Address;
            order.DeliveryType = model.DeliveryType;
            order.PaymentType = model.PaymentType;
            order.Status = model.Status;
            order.AdminNotes = model.AdminNotes;
            order.DeliveryPrice = model.DeliveryPrice;
            order.SubTotal = model.SubTotal;
            order.TotalPrice = model.TotalPrice;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Поръчка #{id} е актуализирана успешно!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            _context.OrderItems.RemoveRange(order.Items);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Поръчка #{id} е изтрита успешно!";
            return RedirectToAction(nameof(Index));
        }

        private static string GetStatusDisplayName(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "Нова поръчка",
            OrderStatus.Processing => "В обработка",
            OrderStatus.Shipped => "Изпратена",
            OrderStatus.Delivered => "Доставена",
            OrderStatus.Cancelled => "Отказана",
            _ => status.ToString()
        };
    }
}
