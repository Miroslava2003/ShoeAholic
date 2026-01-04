using Microsoft.AspNetCore.Mvc;
using ShoeAholic.Data;
using ShoeAholic.Models;

namespace ShoeAholic.Controllers
{
    public class ContactController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public ContactController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessage model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedOn = DateTime.Now;
            model.IsRead = false;

            _context.ContactMessages.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Благодарим Ви! Вашето съобщение беше изпратено успешно.";
            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
