using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Areas.Identity.Data;
using ShoeAholic.Data;
using ShoeAholic.Models.ViewModels;
using System.Security.Claims;

namespace ShoeAholic.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ShoeAholicDbContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ShoeAholicDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ordersCount = await _context.Orders
                .Where(o => o.UserId == userId)
                .CountAsync();

            var totalSpent = await _context.Orders
                .Where(o => o.UserId == userId)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            var model = new ProfileViewModel
            {
                FirstName = user.Име,
                LastName = user.Фамилия,
                Email = user.Email!,
                Phone = user.Телефон,
                OrdersCount = ordersCount,
                TotalSpent = totalSpent
            };

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                FirstName = user.Име,
                LastName = user.Фамилия,
                Email = user.Email!,
                Phone = user.Телефон
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (model.Email != user.Email)
            {
                var emailExists = await _userManager.Users
                    .AnyAsync(u => u.Email == model.Email && u.Id != user.Id);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Този имейл вече е регистриран.");
                    return View(model);
                }
            }

            if (model.Phone != user.Телефон)
            {
                var phoneExists = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == model.Phone && u.Id != user.Id);

                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Този телефонен номер вече е регистриран.");
                    return View(model);
                }
            }

            user.Име = model.FirstName;
            user.Фамилия = model.LastName;
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpper();
            user.UserName = model.Email;
            user.NormalizedUserName = model.Email.ToUpper();
            user.Телефон = model.Phone;
            user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Профилът е обновен успешно!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Паролата е променена успешно!";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                var errorMessage = error.Code switch
                {
                    "PasswordMismatch" => ("CurrentPassword", "Текущата парола е грешна."),
                    _ when error.Description.Contains("at least") => ("NewPassword", "Паролата трябва да е поне 8 символа."),
                    _ when error.Description.Contains("digit") => ("NewPassword", "Паролата трябва да съдържа поне една цифра."),
                    _ when error.Description.Contains("lowercase") => ("NewPassword", "Паролата трябва да съдържа поне една малка буква."),
                    _ when error.Description.Contains("uppercase") => ("NewPassword", "Паролата трябва да съдържа поне една главна буква."),
                    _ when error.Description.Contains("non alphanumeric") => ("NewPassword", "Паролата трябва да съдържа поне един специален символ."),
                    _ => (string.Empty, error.Description)
                };
                ModelState.AddModelError(errorMessage.Item1, errorMessage.Item2);
            }

            return View(model);
        }

        public IActionResult Delete()
        {
            if (User.IsInRole("Admin"))
            {
                TempData["Error"] = "Администраторите не могат да изтриват профила си.";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string password)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Администраторите не могат да изтриват профила си.";
                return RedirectToAction("Index");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordCheck)
            {
                ModelState.AddModelError("Password", "Грешна парола.");
                return View("Delete");
            }

            var userId = user.Id;

            var cartItems = await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
            _context.ShoppingCartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Профилът е изтрит успешно.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View("Delete");
        }
    }
}