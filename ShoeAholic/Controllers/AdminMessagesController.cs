using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;

namespace ShoeAholic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminMessagesController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public AdminMessagesController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string filter = "all", int pageSize = 20, int page = 1)
        {
            var query = _context.ContactMessages.AsQueryable();

            if (filter == "unread")
                query = query.Where(m => !m.IsRead);
            else if (filter == "read")
                query = query.Where(m => m.IsRead);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.Name.Contains(search) ||
                    m.Email.Contains(search) ||
                    m.Phone.Contains(search) ||
                    m.Message.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var messages = await query
                .OrderByDescending(m => m.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalMessages = await _context.ContactMessages.CountAsync();
            ViewBag.UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead);
            ViewBag.ReadMessages = await _context.ContactMessages.CountAsync(m => m.IsRead);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.Search = search;
            ViewBag.Filter = filter;

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            message.IsRead = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Съобщението беше маркирано като прочетено.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            message.IsRead = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Съобщението беше маркирано като непрочетено.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Съобщението беше изтрито.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }
    }
}
